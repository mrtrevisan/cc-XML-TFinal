using Converter;
using Generator;
using Validator;
using Querier;
using Transformer;

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

            string prodBarato = querier.QueryProdMaisBarato(jsonFiles);
            
            Console.WriteLine(prodBarato);
        }

        static void query4(JsonQuerier querier){
            Console.WriteLine("Query 4: Detalhes da nota com maior imposto:");

            string notaMaisTax = querier.QueryNotaMaisTax(jsonFiles);

            Console.WriteLine(notaMaisTax);
        }

        static void transformJson(JsonTransformer transformer, JsonQuerier querier){
            int nNotas = jsonFiles.Length;
            int nProd = 0;
            float tValue = .0f;
            float vIcms = .0f;
            float vFrete = .0f;
            float vTrib = .0f;

            for ( int i = 0; i < jsonFiles.Length; i++){
                string jsonPath = jsonFiles[i];
                string htmlPath = htmlFiles[i];
                transformer.HtmlFromJson(jsonPath, htmlPath, i);

                nProd   += querier.QueryNumProd(jsonFiles[i]);
                tValue  += querier.QueryTotalValue(jsonFiles[i]);
                vIcms   += querier.QueryICMS(jsonFiles[i]);
                vFrete  += querier.QueryFrete(jsonFiles[i]);
                vTrib   += querier.QueryTributes(jsonFiles[i]);
            }

            transformer.HtmlFromData(indexFile, nNotas, nProd, tValue, vIcms, vFrete, vTrib, htmlFiles);       
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
            JsonTransformer transformer = new();
            transformJson(transformer, querier);
        }
    }
}