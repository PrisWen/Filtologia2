using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Windows.Storage;
using Windows.Data.Json;

namespace Fitologia.DataModel
{ 
    //para las partes
    public class SampleDataSubparte
    {
        public SampleDataSubparte(String uniqueId, String title, String imagePath, String description)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Description = description;
            this.ImagePath = imagePath;

        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }


        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem
    {
        public SampleDataItem(String uniqueId, String title, String imagePath, String description)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Subpartes = new ObservableCollection<SampleDataSubparte>();

        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public ObservableCollection<SampleDataSubparte> Subpartes { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup
    {
        public SampleDataGroup(String uniqueId, String title, String imagePath, String description)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Items = new ObservableCollection<SampleDataItem>();
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public ObservableCollection<SampleDataItem> Items { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// PartDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class PartDataSource
    {
        private static PartDataSource partes = new PartDataSource();

        private ObservableCollection<SampleDataGroup> _groups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> Groups
        {
            get { return this._groups; }
        }

        public static async Task<IEnumerable<SampleDataGroup>> GetGroupsAsync()
        {
            await partes.GetSampleDataAsync();

            return partes.Groups;
        }

        public static async Task<SampleDataGroup> GetGroupAsync(string uniqueId)
        {
            await partes.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = partes.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<SampleDataItem> GetItemAsync(string uniqueId)
        {
            await partes.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = partes.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        private async Task GetSampleDataAsync()
        {
            if (this._groups.Count != 0)
                return;

            Uri dataUri = new Uri("ms-appx:///DataModel/Partes.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Groups"].GetArray();

            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                SampleDataGroup group = new SampleDataGroup(groupObject["UniqueId"].GetString(),
                                                            groupObject["Title"].GetString(),
                                                            groupObject["ImagePath"].GetString(),
                                                            groupObject["Description"].GetString());

                foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                {
                    JsonObject itemObject = itemValue.GetObject();

                    SampleDataItem myItem = new SampleDataItem(itemObject["UniqueId"].GetString(),
                                                       itemObject["Title"].GetString(),
                                                       itemObject["ImagePath"].GetString(),
                                                       itemObject["Description"].GetString());
                    foreach (JsonValue subparteValue in itemObject["Subpartes"].GetArray())
                    {
                        JsonObject subparteObject = subparteValue.GetObject();
                        SampleDataSubparte mySubparte = new SampleDataSubparte(subparteObject["UniqueId"].GetString(),
                                                                                subparteObject["Title"].GetString(),
                                                                                 subparteObject["ImagePath"].GetString(),
                                                                                 subparteObject["Description"].GetString());

                        myItem.Subpartes.Add(mySubparte);
                    }

                    group.Items.Add(myItem);
                }
                this.Groups.Add(group);
            }
        }
    }
}