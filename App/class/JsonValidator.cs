using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Validator
{
    class JsonValidator{
        public void Validate(string schemaFile, string[] jsonFiles) {
            //carrega o JSON Schema 
            string jsonSchema = File.ReadAllText(schemaFile);
            //parse do JSON Schema
            JSchema schema = JSchema.Parse(jsonSchema);

            //valida cada arquivo Json
            for (int i = 0; i < 6; i++){   
                try
                {
                    //carregar o arquivo JSON 
                    string JSONpath = jsonFiles[i];
                    string jsonS = File.ReadAllText(JSONpath);

                    //parse do JSON para um objeto JToken
                    JToken jToken = JToken.Parse(jsonS);

                    //valida o JSON com base no schema, os erros serão gravados numa lista
                    bool isValid = jToken.IsValid(schema, out IList<string> messages);

                    if (isValid)
                    {
                        Console.WriteLine($"\t-> O arquivo JSON {i+1} é válido de acordo com o esquema!");
                    }
                    else
                    {
                        Console.WriteLine($"\t-> O arquivo JSON {i+1} NÃO é válido de acordo com o esquema.");
                        foreach (var error in messages)
                        {
                            Console.WriteLine($"\t\t-- Erro: {error}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocorreu um erro: " + ex.Message);
                }
            }
        }
    }
}