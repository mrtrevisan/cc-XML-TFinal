
using System.Text;
using Newtonsoft.Json.Linq;

namespace Transformer
{
    class JsonTransformer
    {
        public void HtmlFromJson(string jsonPath, string htmlPath, int i)
        {
            try
            {
                string json = File.ReadAllText(jsonPath);
                JObject jsonObj = JObject.Parse(json);

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

                File.WriteAllText(htmlPath, htmlBuilder.ToString());
                Console.WriteLine($"\t-> Html da nota {i+1} gerado com sucesso!");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void HtmlFromData
        (
            string htmlPath, 
            int nNotas, 
            int nProd, 
            float tValue, 
            float vIcms, 
            float vFrete, 
            float vTrib,
            string[] htmlFiles
        )
        {
            try {
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

                File.WriteAllText(htmlPath, htmlBuilder.ToString());
                Console.WriteLine($"\t-> Html index gerado com sucesso!");
            }
            catch (Exception) 
            {
                throw;
            }

        }
    }
}