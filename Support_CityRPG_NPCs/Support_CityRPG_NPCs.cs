$City::NPC::DataPath = "Add-Ons/Support_CityRPG_NPCs/data/";
$City::NPC::ScriptPath = "Add-Ons/Support_CityRPG_NPCs/";

exec($City::NPC::ScriptPath @ "cityModules/npcs.cs");
exec($City::NPC::ScriptPath @ "npcSpawn.cs");

package CityRPG_NPC
{
  function City_Tick()
  {
    Parent::City_Tick();

    City_NPCPoseLoop(0);
    City_NPCTickLoop(0);
  }

  function City_TickLoop(%loop)
  {
    Parent::City_TickLoop(%loop);
    %client = findClientByBL_ID(CityRPGData.data[%loop].ID);

    if(isObject(%clientBG = %client.brickGroup))
    {
      if(%clientBG.foodIncome > 0)
      {
        messageClient(%client,'',"\c6 - You received \c3$"@%clientBG.foodIncome@"\c6 from selling \c3"@%clientBG.foodNumPortions@"\c6 portions of food.");
        %clientBG.foodIncome = "";
        %clientBG.foodNumPortions = "";
      }
      if(%clientBG.rentIncome > 0)
      {
        messageClient(%client,'',"\c6 - You received \c3$"@%clientBG.rentIncome@"\c6 from \c3"@%clientBG.rentNumPayments@"\c6 rental payments.");
        %clientBG.rentIncome = "";
        %clientBG.rentNumPayments = "";
      }
      if(%clientBG.itemIncome > 0)
      {
        messageClient(%client,'',"\c6 - You received \c3$"@%clientBG.itemIncome@"\c6 from selling \c3"@%clientBG.itemNumSales@"\c6 items.");
        %clientBG.itemIncome = "";
        %clientBG.itemNumSales = "";
      }
    }
  }
};

deactivatePackage(CityRPG_NPC);
activatePackage(CityRPG_NPC);
