using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DisagReferenceShot
{
    public abstract class XmlParser
    {
        public static List<Shooter>? GetShootersFromXml(string file)
        {
            List<Shooter> shooters = new List<Shooter>();

            XmlNodeList? shooterListXml = ExtractShootersAsNodes(file);

            List<Shot> allShots = GetAllShots(file);


            if (shooterListXml != null)
            {
                foreach (XmlNode shooterNode in shooterListXml)
                {
                    if (shooterNode != null)
                    {
                        string? firstName = shooterNode.Attributes?["firstname"]?.Value;
                        string? lastName = shooterNode.Attributes?["lastname"]?.Value;
                        string? idStr =  shooterNode.Attributes?["idShooters"]?.Value;

                        if (uint.TryParse(idStr, out uint id))
                        {
                            // Match the shooter 
                            if (firstName != null && lastName != null)
                            {

                                // Now find the shots belonging to this shooter
                                IEnumerable<Shot> shootersShots = allShots.Where(s => s.shootersId == id);

                                if (shootersShots.Count() <= 0)
                                {
                                    Console.WriteLine("Shooter without shots found. Skipping.");
                                }
                                else
                                {
                                    shooters.Add(new Shooter(id, firstName, lastName, shootersShots.ToList<Shot>()));
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Could not parse shooters id");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No shooter element found");
                    }
                }
            }

            return shooters;
        }



        public static List<Shot> GetAllShots(string file)
        {
            List<Shot> shots = new List<Shot>();
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(file);
                XmlNodeList shotNodes = doc.GetElementsByTagName("shot");

                foreach (XmlNode shotNode in shotNodes)
                {
                    string? shootersIdStr = shotNode.Attributes?["shootersid"]?.Value;
                    string? shotIdStr = shotNode.Attributes?["shot"]?.Value;
                    string? factorStr = shotNode.Attributes?["factor"]?.Value.Replace('.', ',');
                    string? decStr = shotNode.Attributes?["dec"]?.Value;
                    string? xStr = shotNode.Attributes?["x"]?.Value;
                    string? yStr = shotNode.Attributes?["y"]?.Value;
                    string? divStr = shotNode.Attributes?["orig_teiler"]?.Value.Replace('.', ',');

                    bool conversionValid = uint.TryParse(shootersIdStr, out uint shootersId);
                    conversionValid &= uint.TryParse(shotIdStr, out uint shotId);
                    conversionValid &= double.TryParse(factorStr, out double factor);
                    conversionValid &= double.TryParse(decStr, out double dec);
                    conversionValid &= int.TryParse(xStr, out int xCoord);
                    conversionValid &= int.TryParse(yStr, out int yCoord);
                    conversionValid &= double.TryParse(divStr, out double div);

                    shots.Add(new Shot(shotId, xCoord, yCoord, dec, shootersId, factor, div));
                }
            }
            catch (XmlException ex)
            {
                Console.WriteLine("Could not read XML:" + ex.ToString()); ;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not read XML:" + ex.ToString());
            }

            return shots;
        }


        private static XmlNodeList? ExtractShootersAsNodes(string file)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(file);
                return doc.GetElementsByTagName("shooter");
            }
            catch (XmlException ex)
            {
                Console.WriteLine("Could not read XML:" + ex.ToString()); ;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not read XML:" + ex.ToString());
            }

            return null; 
        }

    }
}
