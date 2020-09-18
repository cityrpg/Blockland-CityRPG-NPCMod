$City::NPC::DataPath = "Add-Ons/Support_CityRPG_NPCs/data/";
$City::NPC::ScriptPath = "Add-Ons/Support_CityRPG_NPCs/";

exec($City::NPC::ScriptPath @ "cityModules/npcs.cs");
exec($City::NPC::ScriptPath @ "npcSpawn.cs");
