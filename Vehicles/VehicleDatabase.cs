using System.Reflection;

namespace Vehicles;

public class VehicleDatabase
{
   private Dictionary<string, List<Vehicle>> vehiclesById;

   public VehicleDatabase()
   {
      vehiclesById = new Dictionary<string, List<Vehicle>>();
      string csvPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ev.csv");
      Load(csvPath);
   }

   /// <summary>
   /// Loads the vehicle data from disk. 
   /// NOTE: the registrations are in chronological order with newest first. That is, the first occurrence of a vehicle
   ///       will be the current registration for that vehicle. Any subsequent occurrence is a previous owner etc.
   /// </summary>
   /// <param name="csvPath"></param>
   private void Load(string csvPath)
   {
      var lines = File.ReadLines(csvPath);
      foreach (string row in lines.Skip(1))
      {
         var columns = row.Split(',');
         string id = columns[0];
         string county = columns[1];
         string city = columns[2];
         string state = columns[3];
         int modelYear = int.Parse(columns[5]);
         string make = columns[6];
         string model = columns[7];
         string evType = columns[8];
         int evRange = int.Parse(columns[10]);
         bool cafvEligible = columns[9] == "Clean Alternative Fuel Vehicle Eligible";

         var entry = new Vehicle { Id = id, State = state, City = city, County = county, Make = make, Model = model, ModelYear = modelYear, EvType = evType, EvRange = evRange, IsCafvEligible = cafvEligible };

         if (vehiclesById.TryGetValue(id, out var list))
         {
            list.Add(entry);
         }
         else
         {
            vehiclesById[id] = new List<Vehicle> { entry };
         }
      }
   }

   public List<List<Vehicle>> GetRegistrations() => vehiclesById.Values.ToList();

   public IEnumerable<Vehicle> GetVehicles() => vehiclesById.Values.Select(x => x[0]);

   public int GetRegistrationCount(string id)
   {
      if (vehiclesById.TryGetValue(id, out var list))
         return list.Count;
      return 0;
   }
}
