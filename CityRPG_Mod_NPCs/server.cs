%error = ForceRequiredAddOn("GameMode_CityRPG4");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: CityRPG4_NPCs - required add-on GameMode_CityRPG4 not found");
  return;
}

exec("./CityRPG4_NPCs.cs");
