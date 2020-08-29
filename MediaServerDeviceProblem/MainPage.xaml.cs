using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MediaServerDeviceProblem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

		private async System.Threading.Tasks.Task<Windows.Storage.StorageFolder> GetFolderWithNameAsync(Windows.Storage.StorageFolder rootFolder, string strFolderName)
		{
			if (rootFolder == null)
				return null;
			var lpSubfolders = await rootFolder.GetItemsAsync();//if jellyfin doesnt return folders with getfoldersasync/only if GetItemsAsync is used
			return lpSubfolders.First((folder) => { return folder.Name == strFolderName; }) as Windows.Storage.StorageFolder;
		}

		private void CheckResultCount(IReadOnlyList<Windows.Storage.IStorageItem> storageItems)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("returned {0} ", storageItems.Count));
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{

			var mediaServerDevices = Windows.Storage.KnownFolders.MediaServerDevices;

			// Get the first child folder, which represents the SD card.
			var lpRootFolders = await mediaServerDevices.GetFoldersAsync();
			foreach (var lpRootFolder in lpRootFolders)// windows 10 dlna uses path root folder\Videos\All Videos for videos
			{
				var folderVideos = await GetFolderWithNameAsync(lpRootFolder, "Videos");

				var folderAllVideos = await GetFolderWithNameAsync(folderVideos, "All Videos");

				if(folderAllVideos!=null)
				{
					//has 406 files in folder(visible through windows explorer)
					CheckResultCount( await folderAllVideos.GetItemsAsync());//returns 200
					CheckResultCount(await folderAllVideos.GetFilesAsync());//returns 200
					CheckResultCount(await folderAllVideos.GetItemsAsync(201,400));//returns 0
					CheckResultCount(await folderAllVideos.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.DefaultQuery, 180,210));//returns 20
				}

			}
		}
	}
}
