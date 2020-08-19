using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediasiteUtil.Models;
using RestSharp;

namespace MediasiteUtil
{
	public partial class MediasiteClient
	{
		public async Task<RecorderStatus> GetRecorderStatusAsync(string recorderId)
		{
			var request = new RestRequest(String.Format("Recorders('{0}')/ExtendedStatus", recorderId), Method.GET);
			var results = await Client.GetAsyncResponse<RecorderStatus>(request);
			ExpectResponse(HttpStatusCode.OK, request, results);

			return results.Data;
		}

		/// <summary>
		/// Start a recorder
		/// </summary>
		/// <param name="recorderId">Recorder Id</param>
		public async void RecorderStartAsync(string recorderId)
		{
			var request = new RestRequest(String.Format("Recorders('{0}')/Start", recorderId), Method.POST);
			var results = await Client.GetAsyncResponse(request);
		}

		/// <summary>
		/// Finds and returns a folder by folder name, exact match
		/// </summary>
		/// <param name="folderId"></param>
		/// <returns></returns>
		public async Task<Folder> FindFolderByIdAsync(string folderId)
		{
			// request the folder
			var request = new RestRequest(String.Format("Folders('{0}')", folderId), Method.GET);
			request.RootElement = "value";
			var folder = await Client.GetAsyncResponse<List<Folder>>(request);

			// check for errors and expected number of results
			ExpectResponse(HttpStatusCode.OK, request, folder);
			ExpectSingleResult(request, folder.Data);
			return folder.Data[0];
		}

		/// <summary>
		/// Finds all folders contained in the given parentFolderId, top level only
		/// </summary>
		/// <param name="parentFolderId"></param>
		/// <returns>A list of folders</returns>
		public async Task<List<Folder>> FindFoldersAsync(string parentFolderId)
		{
			// pagination
			var returned = _batchSize;
			var current = 0;

			// request the folder
			var filter = String.Format("ParentFolderId eq '{0}'", parentFolderId);
			var folders = new List<Folder>();
			while (returned == _batchSize)
			{
				var request = CreatePagedRestRequest("Folders", filter, "Name", _batchSize, current);
				var results = await Client.GetAsyncResponse<List<Folder>>(request);
				ExpectResponse(HttpStatusCode.OK, request, results);
				current += returned = results.Data.Count;
				folders.AddRange(results.Data);
			}

			return folders;
		}

		/// <summary>
		/// Finds all folders contained in the given parent folder, including all subfolders if specified.
		/// </summary>
		/// <param name="parentFolderId"></param>
		/// <param name="includeSubfolders"></param>
		/// <param name="includeParentFolder"></param>
		/// <returns>A non-hierarchical list of Folders</returns>
		public async Task<List<Folder>> FindFoldersAsync(string parentFolderId, bool includeSubfolders, bool includeParentFolder)
		{
			var foundFolders = await FindFoldersAsync(parentFolderId);

			if (!includeSubfolders || foundFolders.Count == 0)
			{
				return foundFolders;
			}

			// recurse through subfolders of each folder returned
			foreach (var folder in foundFolders)
			{
				folder.ChildFolders = await FindFoldersAsync(folder.Id, true, false);
			}

			if (includeParentFolder)
			{
				var parentFolder = await FindFolderByIdAsync(parentFolderId);
				parentFolder.ChildFolders = foundFolders;
				return new List<Folder> { parentFolder };
			}
			return foundFolders;
		}

		/// <summary>
		/// Creates a Presentation
		/// </summary>
		/// <param name="p"></param>
		/// <param name="SelectedRecorder"></param>
		public async Task<PresentationFullRepresentation> CreatePresentationAsync(Recorder recorder, Template template, Folder folder, String playerId, String participants)
		{
			var request = new RestRequest(String.Format("Templates('{0}')/CreatePresentationFromTemplate", template.Id), Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddBody(NewPresentationJson(folder.Id, playerId, participants));
			var results = await Client.GetAsyncResponse<PresentationFullRepresentation>(request);
			ExpectResponse(HttpStatusCode.OK, request, results);
			return results.Data;
		}
	}
}