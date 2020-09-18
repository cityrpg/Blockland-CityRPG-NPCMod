%error = ForceRequiredAddOn("GameMode_CityRPG4");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: Support_CityRPG_NPCs - required add-on GameMode_CityRPG4 not found");
  return;
}

exec("./Support_CityRPG_NPCs.cs");
