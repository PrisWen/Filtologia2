using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace Fitologia.DataModel
{
    public class DataImage
    {
        public string Imagen { get; set; }
        public string Titulo { get; set; }

        public DataImage(String Imagen, String Titulo)
        {
            this.Imagen = Imagen;
            this.Titulo = Titulo;
        }

        public override string ToString()
        {
            return this.Titulo;
        }
    }

    public class DataSubTitulos
    {
        public string Id { get; set; }
        public string Titulo { get; set; }
        public List<string> Contenido { get; set; }
        public List<string> Image { get; set; }

        public DataSubTitulos(String Id, String Titulo, List<String> Contenido, List<String> Image)
        {
            this.Id = Id;
            this.Titulo = Titulo;
            this.Contenido = Contenido;
            this.Image = Image;
        }

        public override string ToString()
        {
            return this.Titulo;
        }
    }

    public class DataTemas
    {
        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public ObservableCollection<DataSubTitulos> Subtitulos { get; set; }

        public DataTemas(String Id, String Titulo, String Descripcion, String Imagen)
        {
            this.Id = Id;
            this.Titulo = Titulo;
            this.Descripcion = Descripcion;
            this.Imagen = Imagen;
            this.Subtitulos = new ObservableCollection<DataSubTitulos>();
        }

        public override string ToString()
        {
            return this.Titulo;
        }
    }

    public sealed class DataSource
    {
        public EventHandler<ObservableCollection<DataTemas>> GetTemaCompleted;

        private ObservableCollection<DataTemas> _temas = new ObservableCollection<DataTemas>();
        public ObservableCollection<DataTemas> Temas
        {
            get { return _temas; }
            set { _temas = value; }
        }

        public ObservableCollection<DataTemas> GetTemasAsync()
        {
            return Temas;
        }

        public DataTemas GetTemasAsync(string Id)
        {
            var matches = Temas.Where((tema) => tema.Id.Equals(Id));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public DataSubTitulos GetSubtituloAsync(string Id)
        {
            var matches = Temas.SelectMany(sub => sub.Subtitulos).Where((item) => item.Id.Equals(Id));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public List<DataImage> GetImageAsync()
        {
            List<DataImage> img = new List<DataImage>();
            foreach (var subtitulos in Temas.SelectMany(sub => sub.Subtitulos))
            {
                img.Add(new DataImage(subtitulos.Image.First(), subtitulos.Titulo));
            }
            return img;
        }

        public async void GetDataAsync()
        {
            Temas = new ObservableCollection<DataTemas>();

            Uri dataUri = new Uri("ms-appx:///DataModel/fitologia.json");
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Temas"].GetArray();

            foreach (JsonValue temaValue in jsonArray)
            {
                JsonObject TemasObject = temaValue.GetObject();
                DataTemas temas = new DataTemas(TemasObject["Id"].GetString(), TemasObject["Titulo"].GetString(), TemasObject["Descripcion"].GetString(), TemasObject["Imagen"].GetString());
                foreach (JsonValue subtituloValue in TemasObject["Subtitulos"].GetArray())
                {
                    JsonObject subtitulo = subtituloValue.GetObject();
                    List<string> cont = new List<string>();
                    foreach (JsonValue contenidoValue in subtitulo["Contenido"].GetArray())
                    {
                        JsonObject contenido = contenidoValue.GetObject();
                        cont.Add(contenido["text"].GetString());
                    }
                    List<String> img = new List<String>();
                    foreach (IJsonValue ImagenValue in subtitulo["Imagenes"].GetArray())
                    {
                        JsonObject image = ImagenValue.GetObject();
                        img.Add(image["url"].GetString());
                    }
                    temas.Subtitulos.Add(new DataSubTitulos(subtitulo["Id"].GetString(), subtitulo["Titulo"].GetString(), cont, img));
                }
                this.Temas.Add(temas);
            }
            if (GetTemaCompleted != null)
            {
                GetTemaCompleted(this, Temas);
            }
        }
    }
}
