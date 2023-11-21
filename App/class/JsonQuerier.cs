using Newtonsoft.Json.Linq;

namespace Querier
{
    class JsonQuerier
    {
        public int QueryNumProd(string JsonPath)
        {            
            try
            {
                string json = File.ReadAllText(JsonPath);
                // Parse do JSON para um objeto JObject
                JObject jsonObj = JObject.Parse(json);

                JToken prods = jsonObj.SelectToken("$.nfeProc.NFe.infNFe.det")!;
                return prods.Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public float QueryTotalValue(string JsonPath)
        {   
            float tValue = .0f;

            try {
                string json = File.ReadAllText(JsonPath);
                // Parse do JSON para um objeto JObject
                JObject jsonObj = JObject.Parse(json);

                JToken prods = jsonObj.SelectToken("$.nfeProc.NFe.infNFe.det")!;

                foreach(var prod in prods){
                    tValue += (float)prod["prod"]!["vProd"]!;
                }

                return tValue;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public float QueryICMS(string JsonPath)
        {
            try {
                string json = File.ReadAllText(JsonPath);
                // Parse do JSON para um objeto JObject
                JObject jsonObj = JObject.Parse(json);

                JToken totais = jsonObj.SelectToken("$.nfeProc.NFe.infNFe.total.ICMSTot")!;
                return (float) totais["vICMS"]!;
              
            }
            catch (Exception)
            {
                throw;
            }
        }

        public float QueryTributes(string JsonPath)
        {
            try {
                string json = File.ReadAllText(JsonPath);
                JObject jsonObj = JObject.Parse(json);

                JToken totais = jsonObj.SelectToken("$.nfeProc.NFe.infNFe.total.ICMSTot")!;
                return (float) totais["vICMS"]! + (float) totais["vIPI"]! + (float) totais["vPIS"]! + (float) totais["vCOFINS"]!;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public float QueryFrete(string JsonPath)
        {
            try {
                string json = File.ReadAllText(JsonPath);
                JObject jsonObj = JObject.Parse(json);

                JToken totais = jsonObj.SelectToken("$.nfeProc.NFe.infNFe.total.ICMSTot")!;
                return (float) totais["vFrete"]!;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string QueryProdMaisBarato (string[] JsonFiles)
        {
            float minValue = float.MaxValue;
            JToken? prodBarato = null;

                try
                {
                    foreach (string JsonPath in JsonFiles){
                        string json = File.ReadAllText(JsonPath);
                        JObject jsonObj = JObject.Parse(json);

                        JToken dets = jsonObj.SelectToken("$.nfeProc.NFe.infNFe.det")!;

                        foreach(var prod in dets){
                            if ( (float) prod["prod"]!["vProd"]! < minValue){
                                prodBarato = prod["prod"]!;
                                minValue = (float) prod["prod"]!["vProd"]!;
                            }
                        }                
                    }
                    return prodBarato!.ToString();
                }
                catch (Exception)
                {
                    throw;           
                }
        }

        public string QueryNotaMaisTax (string[] JsonFiles)
        {
            float maxTax = float.MinValue;
            JToken? notaMaisTax = null;

            try
            {
                foreach (string JsonPath in JsonFiles){
                    string json = File.ReadAllText(JsonPath);
                    JObject jsonObj = JObject.Parse(json);

                    JToken total = jsonObj.SelectToken("$.nfeProc.NFe.infNFe.total.ICMSTot")!;

                    float impNota = (float) total["vICMS"]! + (float) total["vIPI"]! + (float) total["vPIS"]! + (float) total["vCOFINS"]!;

                    if ( impNota > maxTax ){
                        notaMaisTax = total.Parent?.Parent?.Parent?.Parent!["det"];
                        maxTax = impNota;
                    }          
                }
                return notaMaisTax!.ToString();
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}