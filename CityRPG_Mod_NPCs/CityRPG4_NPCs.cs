$CityNPCs::DataPath = "Add-Ons/CityRPG_Mod_NPCs/data/";
$City::NPC::ScriptPath = "Add-Ons/CityRPG_Mod_NPCs/";

exec($City::NPC::ScriptPath @ "cityModules/npcs.cs");
exec($City::NPC::ScriptPath @ "npcSpawn.cs");

//NPC Prefs
$Pref::Server::City::NPC::startPose = true;
$Pref::Server::City::NPC::debug = true;

function npcDebug(%str, %id)
{
  if($Pref::Server::City::NPC::debug)
  {
    if(%id !$= "")
    {
      echo("NPC Mod [" @ %id @ "]: " @ %str);
    }
    else
    {
      echo("NPC Mod: " @ %str);
    }
  }
}

package CityRPG_NPC
{
  // City functions
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

  function City_Init_AssembleEvents()
  {
    Parent::City_Init_AssembleEvents();

    $NPCItemList = "";
    $NPCJobList = "";

    for(%b = 0; %b <= JobSO.getJobCount()-1; %b++)
    {
      %jobObject = JobSO.job[getField(JobSO.jobsIndex, %b)];

      if(strlen(%jobObject.name) > 10)
        %jobName = getSubStr(%jobObject.name, 0, 9) @ ".";
      else
        %jobName = %jobObject.name;

      %newKey = strreplace(%jobName, " ", "") SPC %b;

      // This is fine with an empty space as the first 'key' because we omit the spacing below.
      $NPCJobList = $NPCJobList SPC %newKey;
    }
    for(%c = 0; %c < $City::ItemCount; %c++)
    {
      $NPCItemList = $NPCItemList SPC strreplace($City::Item::name[%c].uiName, " ", "") SPC %c+1;
    }
    registerInputEvent(fxDTSBrick, setNPCData, "And NPCSpawn");
    registerOutputEvent(NPCSpawn, setNPCName, "string 200 200");
    registerOutputEvent(NPCSpawn, setJob, "list" @ $NPCJobList);
    registerOutputEvent(NPCSpawn, setMoney, "string 200 50");//String because we don't want to cap NPC money.
    registerOutputEvent(NPCSpawn, setHunger, "int 0 " @ $CityRPG::food::stateCount @" 6");
    registerOutputEvent(NPCSpawn, setGreed, "string 200 50" TAB "list NONE 0" @ $NPCItemList TAB "string 200 50" TAB "int 0 200 0");//[price (not capped)], [item], [selling brick OBJECT ID], [selling brick EVENT NUM]
    registerOutputEvent(NPCSpawn, setRent, "string 200 50" TAB "string 200 50");
    //registerOutputEvent(NPCSpawn, setAvatar, "string 200 200") //We could go with a setAvatar event that represents everything as numbers.  But i'd like this to be uh... usable if you don't have the client.
    registerOutputEvent(NPCSpawn, setFootNodes, "list NONE 0 LShoe 1 LPeg 2 SkirtTrimLeft 3" TAB "list NONE 0 RShoe 1 RPeg 2 SkirtTrimRight 3" TAB "list NONE 0 LSki 1" TAB "list NONE 0 RSki 1");
    registerOutputEvent(NPCSpawn, setBodyNodes, "list NONE 0 Pants 1 SkirtHip 2" TAB "list NONE 0 Chest 1 FemChest 2" TAB "list NONE 0 Armor 1 Bucket 2 Cape 3 Pack 4 Quiver 5 Tank 6" TAB "list NONE 0 Epaulets 1 EpauletsRankA 2 EpauletsRankB 3 EpauletsRankC 4 EpauletsRankD 5 ShoulderPads 6");
    registerOutputEvent(NPCSpawn, setArmNodes, "list NONE 0 LArm 1 LArmSlim 2" TAB "list NONE 0 RArm 1 RArmSlim 2" TAB "list NONE 0 LHand 1 LHook 2" TAB "list NONE 0 RHand 1 RHook 2");
    registerOutputEvent(NPCSpawn, setHeadNodes, "list NONE 0 HeadSkin 1" TAB "list NONE 0 Helmet 1 PointyHelmet 2 FlareHelmet 3 ScoutHat 4 Bicorn 5 CopHat 6 KnitHat 7 Hair 8" TAB "list NONE 0 Plume 1 TriPlume 2 SeptPlume 3 Visor 4");
    registerOutputEvent(NPCSpawn, setFootColors, "string 200 90" TAB "string 200 90" TAB "string 200 90" TAB "string 200 90");
    registerOutputEvent(NPCSpawn, setBodyColors, "string 200 90" TAB "string 200 90" TAB "string 200 90" TAB "string 200 90");
    registerOutputEvent(NPCSpawn, setArmColors, "string 200 90" TAB "string 200 90" TAB "string 200 90" TAB "string 200 90");
    registerOutputEvent(NPCSpawn, setHeadColors, "string 200 90" TAB "string 200 90" TAB "string 200 90");
    registerOutputEvent(NPCSpawn, setDecals, "list Smiley 0 Jamie 1 AdamSavage 2 Orc 3 Male07Smiley 4 KleinerSmiley 5 ChefSmiley 6 BrownSmiley 7 SmileyRedBeard2 8 SmileyRedBeard 9 SmileyPirate3 10 SmileyPirate2 11 SmileyPirate1 12 SmileyOld 13 SmileyFemale1 14 SmileyEvil2 15 SmileyEvil1 16 SmileyCreepy 17 SmileyBlonde 18 MemeYaranika 19 MemePBear 20 MemeHappy 21 MemeGrinMan 22 MemeDesu 23 MemeCats 24 MemeBlockMongler 25 AsciiTerror 26 AAA-None 27" TAB "list AAA-None 0 Worm_engineer 1 Worm-sweater 2 LinkTunic 3 Knight 4 HCZombie 5 DrKleiner 6 DKnight 7 Chef 8 Archer 9 Alyx 10 Hoodie 11 Space-Old 12 Space-New 13 Space-Nasa 14 Mod-Suit 15 Mod-Prisoner 16 Mod-Police 17 Mod-Pilot 18 Mod-DareDevil 19 Mod-Army 20 Meme-Mongler 21 Medieval-YARLY 22 Medieval-Tunic 23 Medieval-Rider 24 Medieval-ORLY 25 Medieval-Lion 26 Medieval-Eagle 27");
    registerOutputEvent(NPCSpawn, setPose, "string 200 40" TAB "string 200 200" TAB "bool" TAB "string 200 50");//time [00:00], [posX posY posZ axisX axisY axisZ angle], [animation?], [animation name]
    registerOutputEvent(NPCSpawn, Finish);//When added to a brick, tells the NPC Data manager that you have finished adding NPC events.

    //For sell event tracking.  We only want to record these once.
    $City::Event::sellFood = outputEvent_GetOutputEventIdx("fxDTSBrick","sellFood");
    $City::Event::sellItem = outputEvent_GetOutputEventIdx("fxDTSBrick","sellItem");

    //For $loadoffset modifications.  We want to modify the position data of these events.
    $City::Event::setPose = outputEvent_GetOutputEventIdx("NPCSpawn","setPose");
    $City::Event::setRent = outputEvent_GetOutputEventIdx("NPCSpawn","setRent");

    //For data tracking we want to record these event ids.
    $City::Event::NPCFinish = outputEvent_GetOutputEventIdx("NPCSpawn","Finish");
    $City::Event::setNPCData = inputEvent_getInputEventIDx("setNPCData");
    $City::Event::setNPCData["And"] = inputEvent_GetTargetIndex("fxDTSBrick",$City::Event::setNPCData,"And");

    npcDebug("Assembled events.");
  }

  // Base game functions
  function onMissionLoaded()
  {
    Parent::onMissionLoaded();
		if(!isObject(CityRPGEventTracker))
		{
			%tracker = new SimGroup("CityRPGEventTracker"){};
			%tracker.sellFood = %groupa = new SimGroup(){};
			%tracker.sellItem = %groupb = new SimGroup(){};
			%tracker.add(%groupa, %groupb);
			MissionCleanup.add(%tracker);
		}

		if(!isObject(NPCSpawnList))
		{
			%list = new SimSet("NPCSpawnList"){};
			MissionCleanup.add(%list);
		}
		if(!isObject(NPCProcessList))
		{
			%list = new SimSet("NPCProcessList"){};
			MissionCleanup.add(%list);
		}
		if(!isObject(NPCProcessList_Tick))
		{
			%list = new SimSet("NPCProcessList_Tick"){};
			MissionCleanup.add(%list);
		}
  }

  //Make "events" greyed out on uneditable bricks.
  function WrenchImage::onHitObject(%this, %player, %slot, %object, %pos, %normal)
  {
    if(!isObject(%object))
      return Parent::onHitObject(%this, %player, %slot, %object, %pos, %normal);
    if(%object.getClassName()!$="fxDTSBrick")
      return Parent::onHitObject(%this, %player, %slot, %object, %pos, %normal);
    if(%object.getDataBlock().CityRPGBrickEventsLocked)
    {
      %adminonly = $Server::WrenchEventsAdminOnly;
      $Server::WrenchEventsAdminOnly = 1;
      %value = Parent::onHitObject(%this, %player, %slot, %object, %pos, %normal);
      $Server::WrenchEventsAdminOnly = %adminonly;
    }
    else
    {
      %value = Parent::onHitObject(%this, %player, %slot, %object, %pos, %normal);
    }
    return %value;
  }

  // Server commands
  function serverCmdClearEvents(%client)//Make some bricks uneditable by non-admins.
  {
    %brick = %client.wrenchbrick;
    if(!isObject(%brick))
      return Parent::serverCmdClearEvents(%client);
    if(%brick.getDataBlock().CityRPGBrickEventsLocked)
    {
      if(!%client.isAdmin)
      {
        %client.centerPrint("\c6Only administrators may edit events on this brick.",5);
        return 0;
      }
    }
    %value = Parent::serverCmdClearEvents(%client);

    //Do some event tracking work...
    if(%brick.numEvents == 0)//Success check
    {
      %blid = %brick.getGroup().bl_id;//If a brick changes ownership, it will be in the wrong set.  But afaik you can't sell lots yet, so I can't fix that yet.
      if(isObject(%set = CityRPGEventTracker.sellFood.event["Set",%blid]))
      {
        if(%set.isMember(%brick))
          %set.remove(%brick);
      }
      if(isObject(%set = CityRPGEventTracker.sellItem.event["Set",%blid]))
      {
        if(%set.isMember(%brick))
          %set.remove(%brick);
      }
      if(isObject(%brick.NPCAvatar))
        %brick.NPCAvatar.delete();
      if(NPCProcessList.isMember(%brick))
        NPCProcessList.remove(%brick);
      if(NPCProcessList_Tick.isMember(%brick))
        NPCProcessList_Tick.remove(%brick);
    }

    return %value;
  }

  function serverCmdAddEvent(%client, %enabled, %inputEventIdx, %delay, %targetIdx, %NTNameIdx, %outputEventIdx, %par1, %par2, %par3, %par4)//Make some bricks uneditable by non-admins.
  {
    %brick = %client.wrenchbrick;
    if(!isObject(%brick))
      return Parent::serverCmdAddEvent(%client, %enabled, %inputEventIdx, %delay, %targetIdx, %NTNameIdx, %outputEventIdx, %par1, %par2, %par3, %par4);
    if(%brick.getDataBlock().CityRPGBrickEventsLocked)
    {
      if(!%client.isAdmin&&!%client.isStaff)
      {
        return 0;
      }
    }

    //Apply load offset to pose or rent positions if we are loading a file.
    if(InputEvent_getTargetClass("fxDTSBrick",%inputEventIdx,%targetIdx) $= "NPCSpawn" && (%db = %brick.getDatablock()).getName() $= "brickNPCSpawnData")
    {
      if(%outputEventIDx == $City::Event::setPose)
      {
        if(isObject($Server_LoadFileObj))
        {
          if($LastLoadedBrick == %brick&&$Server_LoadFileObj.getClassName()$="FileObject")//The duplicator will fail this check, as the "fileobject" is actually set to a brick. ;d
          {
            %par2 = VectorAdd(getWords(%par2,0,2),$loadOffset) SPC getWords(%par2,3,6);
          }
        }
      }
      if(%outputEventIDx == $City::Event::setRent)
      {
        if(isObject($Server_LoadFileObj))
        {
          if($LastLoadedBrick == %brick&&$Server_LoadFileObj.getClassName()$="FileObject")//The duplicator will fail this check, as the "fileobject" is actually set to a brick. ;d
          {
            %par2 = VectorAdd(getWords(%par2,0,2),$loadOffset) SPC VectorAdd(getWords(%par2,3,5),$loadOffset) SPC getWord(%par2,6);
          }
        }
      }
      if(%outputEventIDx == $City::Event::NPCFinish)//Avatar creation not triggered when brick is created.  Consider adding Finish Event through script.
      {
        if(%brick.numEvents>=15)
        {
          %db.createAvatar(%brick);
          NPCProcessList.add(%brick);
        }
      }
    }

    %value = Parent::serverCmdAddEvent(%client, %enabled, %inputEventIdx, %delay, %targetIdx, %NTNameIdx, %outputEventIdx, %par1, %par2, %par3, %par4);

    //Some event tracking for NPCs  (They gotta know where to buy stuff)
    %el = %brick.numEvents-1;
    if(%brick.eventOutputIdx[%el] == $City::Event::sellFood&&InputEvent_getTargetClass("fxDTSBrick",%brick.eventInputIdx[%el],%brick.eventTargetIdx[%el]) $= "fxDTSBrick")
    {
      if(%par3<=26)//Ensure the price isn't too high >.<
      {
        %blid = %brick.getGroup().bl_id;
        if(!isObject(%set = CityRPGEventTracker.sellFood.event["Set",%blid]))
        {
          CityRPGEventTracker.sellFood.event["Set",%blid] = %set = new SimSet(){};
          CityRPGEventTracker.sellFood.add(%set);
        }
        %set.add(%brick);
      }
    }
    if(%brick.eventOutputIdx[%el] == $City::Event::sellItem&&InputEvent_getTargetClass("fxDTSBrick",%brick.eventInputIdx[%el],%brick.eventTargetIdx[%el]) $= "fxDTSBrick")
    {
      %blid = %brick.getGroup().bl_id;
      if(!isObject(%set = CityRPGEventTracker.sellItem.event["Set",%blid]))
      {
        CityRPGEventTracker.sellItem.event["Set",%blid] = %set = new SimSet(){};
        CityRPGEventTracker.sellItem.add(%set);
      }
      %set.add(%brick);
    }

    return %value;
  }
};

deactivatePackage(CityRPG_NPC);
activatePackage(CityRPG_NPC);
