using Converter;
using Generator;
using Validator;
using Querier;

using System.Text;
using Newtonsoft.Json.Linq;
using NJsonSchema;

namespace trabalhoFinal
{   
    class Program
    {
        //path dos arquivos xml
        static readonly string[] xmlFiles = {
            "xml/nota1.xml",
            "xml/nota2.xml",
            "xml/nota3.xml",
            "xml/nota4.xml",
            "xml/nota5.xml",
            "xml/nota6.xml"
        };

        //path dos arquivos json
        static readonly string[] jsonFiles = {
            "json/nota1.json",
            "json/nota2.json",
            "json/nota3.json",
            "json/nota4.json",
            "json/nota5.json",
            "json/nota6.json"
        };

        static readonly string[] htmlFiles = {
            "html/nota1.html",
            "html/nota2.html",
            "html/nota3.html",
            "html/nota4.html",
            "html/nota5.html",
            "html/nota6.html"
        };

        static readonly string indexFile = "./index.html";

        static readonly string schemaFile = "json/schema.json";

        static void query1(JsonQuerier querier){
            Console.WriteLine("Query 1: (a)Número de produtos em todas as notas e (b)Valor total dos produtos:");
            int nProd = 0;
            float tValue = .0f;

            foreach(string jsonFile in jsonFiles)
            {
                nProd += querier.QueryNumProd(jsonFile);
                tValue += querier.QueryTotalValue(jsonFile);
            }
            
            Console.WriteLine($"\ta) Total de produtos: {nProd}.\n\tb) Valor total dos produtos: {tValue}");
        }

        static void query2(JsonQuerier querier){
            Console.WriteLine("Query 2: (a)Total do ICMS, (b)Valor aproximado de tributos e (c)Total de frete dos produtos:");
            
            float vIcms = .0f;
            float vTrib = .0f;
            float vFrete = .0f;

            foreach(string jsonFile in jsonFiles)
            {
                vIcms += querier.QueryICMS(jsonFile);
                vTrib += querier.QueryTributes(jsonFile);
                vFrete += querier.QueryFrete(jsonFile);
            }
            
            Console.WriteLine($"\ta) Total de ICSM: {vIcms}.\n\tb) Valor aproximado dos tributos: {vTrib}\n\tc) Valor total de frete: {vFrete}.");
        }

        static void query3(JsonQuerier querier){
            Console.WriteLine("Query 3: Detalhes do produto com menor preço:");

            
            Console.WriteLine(prodBarato);
        }

        static void query4(JsonQuerier querier){
            Console.WriteLine("Query 4: Detalhes da nota com maior imposto:");

            Console.WriteLine(notaMaisTax);
        }

        static void createHTML(JObject jsonObj, int i, float vIcms, float vTrib)
        {
            // Criar um documento HTML simples com base nos dados do JSON
            StringBuilder htmlBuilder = new StringBuilder();
            string data = jsonObj.SelectToken(".nfeProc.NFe.infNFe.ide.dEmi")!.ToString();
            float vNF = jsonObj.SelectToken(".nfeProc.NFe.infNFe.total.ICMSTot.vNF")!.Value<float>();

            
            // Adicionar cabeçalho do HTML
            htmlBuilder.AppendLine("<!DOCTYPE html>");
            htmlBuilder.AppendLine("<html>");
            htmlBuilder.AppendLine($"<head><title>Nota Fiscal {i+1}</title></head>");
            htmlBuilder.AppendLine("<body>");

            // Iterar sobre os objetos JSON e adicionar ao HTML
            htmlBuilder.AppendLine($"\t<h2>Nota fiscal {i+1}:</h2>");
            htmlBuilder.AppendLine($"\t<p>Data de compra: {data}</p>");
            htmlBuilder.AppendLine($"\t<h4>Produtos:</h4>");

            var dets = jsonObj.SelectToken(".nfeProc.NFe.infNFe.det");
            foreach (var prod in dets!)
            {
                htmlBuilder.AppendLine("\t<div>");
                htmlBuilder.AppendLine($"\t\t<p>Nome: {prod["prod"]!["xProd"]!.ToString()}</p>");
                htmlBuilder.AppendLine($"\t\t<p>Valor unitário: {prod["prod"]!["vProd"]!.ToString()}</p>");
                htmlBuilder.AppendLine("\t</div>");
            }
            htmlBuilder.AppendLine($"\t<p>Valor total da nota: {vNF}</p>");

            // Fechar o corpo e a tag HTML
            htmlBuilder.AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");

            File.WriteAllText(htmlFiles[i], htmlBuilder.ToString());
        }

        static void createHTMLindex(int nNotas, int nProd, float tValue, float vIcms, float vFrete, float vTrib){
            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder.AppendLine("<!DOCTYPE html>");
            htmlBuilder.AppendLine("<html>");
            htmlBuilder.AppendLine("<head><title>Notas Fiscais</title></head>");
            htmlBuilder.AppendLine("<body>");

            // Iterar sobre os objetos JSON e adicionar ao HTML
            htmlBuilder.AppendLine("\t<h2>Notas fiscais:</h2>");
            htmlBuilder.AppendLine($"\t<p>Número de notas: {nNotas}</p>");
            htmlBuilder.AppendLine($"\t<p>Número de produtos: {nProd}</p>");

            htmlBuilder.AppendLine($"\t<p>Valor total dos produtos: {tValue}</p>");
            htmlBuilder.AppendLine($"\t<p>Valor total do ICMS: {vIcms}</p>");
            htmlBuilder.AppendLine($"\t<p>Valor total de tributos: {vTrib}</p>");
            htmlBuilder.AppendLine($"\t<p>Valor total de frete: {vFrete}</p>");
            
            htmlBuilder.AppendLine("\t<h3>Notas:</h3>");
            for (int i = 0; i < 6; i++){
                htmlBuilder.AppendLine($"\t<a href=\"{htmlFiles[i]}\">Nota {i+1}</a><br/><br/>");
            }

            // Fechar o corpo e a tag HTML
            htmlBuilder.AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");

            File.WriteAllText(indexFile, htmlBuilder.ToString());
        }      


        static void transformJson(){
            int nNotas = jsonFiles.Length;
            int nProd = 0;
            float tValue = .0f;
            float vIcms = .0f;
            float vFrete = .0f;
            float vTrib = .0f;

            for (int i = 0; i < 6; i++){
                try
                {
                    string jsonPath = jsonFiles[i];
                    string htmlPath = htmlFiles[i];

                    string json = File.ReadAllText(jsonPath);
                    // Parse do JSON para um objeto JObject
                    JObject jsonObj = JObject.Parse(json);

                    //numero de produtos e valor dos produtos
                    
                    JToken prods = jsonObj.SelectToken("$.nfeProc.NFe.infNFe.det")!;
                    nProd += prods.Count();
                    foreach(var prod in prods){
                        tValue += (float)prod["prod"]!["vProd"]!;
                    }
                    //total de imposto
                    JToken total = jsonObj.SelectToken("$.nfeProc.NFe.infNFe.total.ICMSTot")!;
                    vFrete += (float) total["vFrete"]!;

                    float IcmsNota = (float) total["vICMS"]!;
                    vIcms += IcmsNota;
                    
                    float TribNota = (float) total["vICMS"]! + (float) total["vIPI"]! + (float) total["vPIS"]! + (float) total["vCOFINS"]!;
                    vTrib += TribNota;
                    createHTML(jsonObj, i, IcmsNota, TribNota);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocorreu um erro: " + ex.Message);
                }
            }
            createHTMLindex(nNotas, nProd, tValue, vIcms, vFrete, vTrib);       

        }

        static void Main(string[] args)
        {
            

            Console.WriteLine("Conversão dos arquivos XML para JSON:");
            XmlConverter converter = new();
            converter.Convert(xmlFiles, jsonFiles);

            Console.WriteLine("-\nGeração do arquivo JSON Schema:");
            SchemaGenerator generator = new();
            generator.Generate(xmlFiles[0], schemaFile);


            Console.WriteLine("-\nValidação dos arquivos JSON:");
            JsonValidator validator = new();
            validator.Validate(schemaFile, jsonFiles);

            Console.WriteLine("Queries no JSON:");
            JsonQuerier querier = new();

            query1(querier);
            query2(querier);
            query3(querier);
            query4(querier);

            Console.WriteLine("Transformação no JSON");
            transformJson();
        }
    }
}