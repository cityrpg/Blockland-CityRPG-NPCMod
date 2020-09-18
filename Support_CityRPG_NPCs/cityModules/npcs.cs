//NPCs

//Load avatar shape Data
datablock staticShapeData(SSNPCAvatarData)
{
	shapefile = $City::DataPath @ "/shapes/NPC.dts";
	head[0] = "NONE";
	head[1] = "headSkin";

	hat[0] = "NONE";
	hat[1] = "Helmet";
	hat[2] = "PointyHelmet";
	hat[3] = "FlareHelmet";
	hat[4] = "ScoutHat";
	hat[5] = "Bicorn";
	hat[6] = "CopHat";
	hat[7] = "KnitHat";
	hat[8] = "Hair";

	accent[0] = "NONE";
	accent[1] = "Plume";
	accent[2] = "TriPlume";
	accent[3] = "SeptPlume";
	accent[4] = "Visor";

	pack[0] = "NONE";
	pack[1] = "Armor";
	pack[2] = "Bucket";
	pack[3] = "Cape";
	pack[4] = "Pack";
	pack[5] = "Quiver";
	pack[6] = "Tank";

	secondpack[0] = "NONE";
	secondpack[1] = "Epaulets";
	secondpack[2] = "EpauletsRankA";
	secondpack[3] = "EpauletsRankB";
	secondpack[4] = "EpauletsRankC";
	secondpack[5] = "EpauletsRankD";
	secondpack[6] = "ShoulderPads";

	chest[0] = "NONE";
	chest[1] = "Chest";
	chest[2] = "FemChest";

	larm[0] = "NONE";
	larm[1] = "LArm";
	larm[2] = "LArmSlim";

	rarm[0] = "NONE";
	rarm[1] = "RArm";
	rarm[2] = "RArmSlim";

	lhand[0] = "NONE";
	lhand[1] = "LHand";
	lhand[2] = "LHook";

	rhand[0] = "NONE";
	rhand[1] = "RHand";
	rhand[2] = "RHook";

	hip[0] = "NONE";
	hip[1] = "Pants";
	hip[2] = "SkirtHip";

	lleg[0] = "NONE";
	lleg[1] = "LShoe";
	lleg[2] = "LPeg";
	lleg[3] = "SkirtTrimLeft";

	rleg[0] = "NONE";
	rleg[1] = "RShoe";
	rleg[2] = "RPeg";
	rleg[3] = "SkirtTrimRight";

	lski[0] = "NONE";
	lski[1] = "LSki";
	rski[0] = "NONE";
	rski[1] = "RSki";
};
addExtraResource($City::DataPath @ "/shapes/decal.ifl");
addExtraResource($City::DataPath @ "/shapes/face.ifl");

function serverCmdNPCatMe(%client, %val)//This is a temporary function to test avatars ingame.
{
	if(!%client.isAdmin)
		return;
	if(!isObject(%player = %client.player))
		return;
	if(isObject(%client.NPCavatar))
		%client.NPCavatar.delete();
	if(%val)
		return;

	%client.NPCavatar = %shape = new StaticShape("SSTestShape_"@%x@"_"@%y@"_"@%z)
	{
		position = %player.position;
		rotation = %player.rotation;
		scale = "1 1 1";
		dataBlock = "SSNPCAvatarData";
		cansetIFLS = "1";//This shape has IFLS, so we want this on for once >.<
	};
	MissionCleanup.add(%shape);
	HideAllNodes(%shape);//We may need to replace this function if we remove some default nodes.
	%shape.hideNode("Hair");
	//Default Blockhead avatar
	%shape.unhideNode("LShoe");
	%shape.unhideNode("RShoe");
	%shape.unhidenode("pants");
	%shape.unhidenode("chest");
	%shape.unhidenode("Larm");
	%shape.unhidenode("Lhand");
	%shape.unhidenode("Rarm");
	%shape.unhidenode("RHand");
	%shape.unhidenode("HeadSkin");
	//	%shape.playthread(0,"headUp"); //For use with backpacks, capes, epaulets, etc.
	%shape.setNodeColor("LShoe","0 0 1 1");
	%shape.setNodeColor("RShoe","0 0 1 1");
	%shape.setNodeColor("pants","0 0 1 1");
	%shape.setNodeColor("chest","0.9 0.9 0.9 1");
	%shape.setNodeColor("Larm","0.9 0 0 1");
	%shape.setNodeColor("Lhand","1 0.878431 0.611765 1");
	%shape.setNodeColor("Rarm","0.9 0 0 1");
	%shape.setNodeColor("Rhand","1 0.878431 0.611765 1");
	%shape.setNodeColor("HeadSkin","1 0.878431 0.611765 1");
	%shape.setIflFrame("face",0);
	%shape.setIflFrame("decal",0);
	talk("Created NPC Avatar("@%shape.getID()@") at "@%player.position);
}

//Dummy functions for NPC Data storage events.
function NPCSpawn::setNPCName(%this, %client){}
function NPCSpawn::setJob(%this, %client){}
function NPCSpawn::setMoney(%this, %client){}
function NPCSpawn::setHunger(%this, %client){}
function NPCSpawn::setRent(%this, %client){}
function NPCSpawn::setGreed(%this, %client){}
function NPCSpawn::setFootNodes(%this,%client){}
function NPCSpawn::setBodyNodes(%this,%client){}
function NPCSpawn::setArmNodes(%this,%client){}
function NPCSpawn::setHeadNodes(%this,%client){}
function NPCSpawn::setFootColors(%this,%client){}
function NPCSpawn::setBodyColors(%this,%client){}
function NPCSpawn::setArmColors(%this,%client){}
function NPCSpawn::setHeadColors(%this,%client){}
function NPCSpawn::setPose(%this, %client){}
function NPCSpawn::finish(%this, %client){}

////////////////////////////
//CLIENT REQUEST FUNCTIONS//
////////////////////////////

//Request the list of NPC Spawns
//Timeout: 5000 ms
function serverCmdRequestNPCSpawnList(%client)
{
	if(!%client.isAdmin&&!%client.isStaff)
		return;
	if(getSimTime()-%client.NPCSpawnListTimeout<5000)
		return;
	if(!isObject(%list = NPCSpawnList))
		return;
	%client.NPCSpawnListTimeout = getSimTime();
	cancel(%client.NPCSpawnListSched);
	%client.sendNPCSpawnList_Tick(%list,0);
}

//The list will be sent in "pages" which contain multiple spawns.
//Originally I meant to send 100 spawns per page.
//But string length limitations put me at 3 spawns per page.
function GameConnection::sendNPCSpawnList_Tick(%client,%list,%index)
{
	cancel(%client.NPCSpawnListSched);

	%count = %list.getCount();
	for(%i=%index;%i<%count;%i++)
	{
		%brick = %list.getObject(%i);
		%str = %str@(%x?"\n":"")@%brick.getID() TAB getSubStr(%brick.eventOutputParameter[0,1],0,20) TAB getSubStr(%brick.getGroup().name,0,20) TAB %brick.position;
		%x++;
		if(%x>=3)
		{
			%client.NPCSpawnListSched = %client.schedule(10,"sendNPCSpawnList_Tick",%list,%i+1);
			break;
		}
	}
	if(%str!$="")
		commandToClient(%client,'receiveSpawnList',%str,mCeil(%i/3),mCeil(%count/3));
}

//Request the NPC in front of the client's Player
//Timeout: 200 ms
//This request is sent when the client opens their GUI
//They are requestiong that we check if an NPC Spawn is in front of them.
//If there is, please send it's OBJ ID to them.
function serverCmdRequestNPCSpawnSelect(%client)
{
	if(!%client.isAdmin&&!%client.isStaff)
		return;
	if(getSimTime()-%client.NPCSpawnSelectTimeout<200)
		return;
	%client.NPCSpawnSelectTimeout = getSimTime();

	if(!isObject(%player = %client.player))
		return;
	if(%client.getControlObject() != %player)
		return;

	%cast = containerRayCast(%player.getEyePoint(),VectorAdd(%player.getEyePoint(),vectorScale(%player.getEyeVector(),5)),$TypeMasks::FxBrickAlwaysObjectType);
	if(!isObject(getWord(%cast,0)))
		return;
	if(getWord(%cast,0).getDatablock().getName()$="brickNPCSpawnData")
		commandToClient(%client,'selectNPCSpawn',getWord(%cast,0));
}

//Request that we set the client's camera to Orbit a brick.
//Timeout: 0 ms
//This request is sent when the client selects a brick.
//This is used to make it easier to manage NPC Spawns remotely.
function serverCmdRequestOrbitNPCSpawn(%client,%brick)
{
	if(!%client.isAdmin&&!%client.isStaff)
		return;
	if(!isObject(%brick))
		return;
	if(%brick.getClassName()!$="fxDTSBrick")
		return;
	if(!isObject(%camera = %client.camera))
	{
		%client.camera = %camera = new Camera() {
		   position = "0 0 0";
		   rotation = "1 0 0 0";
		   scale = "1 1 1";
		   dataBlock = "Observer";
		   canSetIFLs = "0";
		};
	}

	%client.setControlObject(%camera);
	%camera.setOrbitPointMode(vectorAdd(%brick.position,"0 0 0.1"),2.5);
}

//Request that we set the client's control back to their player object.
//Timeout: 0 ms
//This request is sent when a client requests data about their player,
//or when they close the gui.
function serverCmdRequestResetFromNPCOrbit(%client)
{
	if(!%client.isAdmin&&!%client.isStaff)
		return;
	if(!isObject(%player = %client.player))
		return;
	if(%client.getControlObject()==%client.camera)
		%client.setControlObject(%player);
}

//Request that we teleport the client's player to the specified brick.
//Timeout: 0 ms
//This request is sent when the client clicks the camera box.
//If the client is an administrator, we teleport them to the requested brick.
//It is meant to be used as a quick way for server administrators to reach a specific NPC's spawn point.
function serverCmdRequestTPToOrbit(%client,%object)
{
	if(!%client.isAdmin)
		return;
	if(!isObject(%player = %client.player))
		return;
	if(!isObject(%object))
		return;
	%player.dismount();
	%pos = %object.position;
	%player.teleportEffect();
	%player.setTransform(setWord(%pos,2,getWord(%pos,2)+0.1) SPC "0 0 1" SPC $pi/2*((%object.angleID-1)%4));
	%player.teleportEffect();
}

//Request that we send the client the NPC Data for the specified brick.
//Timeout: 1000 ms
//This request is sent when an NPC is selected from the list.
function serverCmdRequestNPCData(%client,%brick)
{
	if(!%client.isAdmin&&!%client.isStaff)
		return;
	if(getSimTime()-%client.NPCSpawnDataTimeout<1000)
		return;
	if(!isObject(%brick))
		return;
	if(%brick.getClassName()!$="fxDTSBrick")
		return;
	if(%brick.getDatablock().getName()!$="brickNPCSpawnData")
		return;

	%client.NPCSpawnDataTimeOut = getSimTime();

	%client.hasJobList = 0;
	if(!%client.hasJobList)
	{
		for(%j = 1; isObject(JobSO.job[%j]); %j++)
		{
			if(strlen(JobSO.job[%j].name) > 10)
				%jobName = getSubStr(JobSO.job[%j].name, 0, 9) @ ".";
			else
				%jobName = JobSO.job[%j].name;

			commandToClient(%client,'receiveNPCData',%brick,"JOBLIST",strreplace(%jobName, " ", "") TAB %j-1);
		}
		%client.hasJobList = 1;
	}

	commandToClient(%client,'receiveNPCData',%brick,"NAME",%brick.eventOutputParameter[0,1]);
	commandToClient(%client,'receiveNPCData',%brick,"JOB",%brick.eventOutputParameter[1,1]);
	commandToClient(%client,'receiveNPCData',%brick,"MONEY",%brick.eventOutputParameter[2,1]);
	commandToClient(%client,'receiveNPCData',%brick,"HUNGER",%brick.eventOutputParameter[3,1]);
	commandToClient(%client,'receiveNPCData',%brick,"GREED",%brick.eventOutputParameter[4,1] TAB ((%brick.eventOutputParameter[4,2]>0)?strreplace($CityRPG::prices::weapon::name[%brick.eventOutputParameter[4,2]-1].uiName, " ", ""):"NONE") TAB %brick.eventOutputParameter[4,1] TAB %brick.eventOutputParameter[4,2] TAB %brick.eventOutputParameter[4,3] TAB %brick.eventOutputParameter[4,4]);
	commandToClient(%client,'receiveNPCData',%brick,"RENT",%brick.eventOutputParameter[5,1] TAB %brick.eventOutputParameter[5,2]);
	commandToClient(%client,'receiveNPCData',%brick,"FEETN",%brick.eventOutputParameter[6,1] TAB %brick.eventOutputParameter[6,2] TAB %brick.eventOutputParameter[6,3] TAB %brick.eventOutputParameter[6,4]);
	commandToClient(%client,'receiveNPCData',%brick,"BODYN",%brick.eventOutputParameter[7,1] TAB %brick.eventOutputParameter[7,2] TAB %brick.eventOutputParameter[7,3] TAB %brick.eventOutputParameter[7,4]);
	commandToClient(%client,'receiveNPCData',%brick,"ARMN",%brick.eventOutputParameter[8,1] TAB %brick.eventOutputParameter[8,2] TAB %brick.eventOutputParameter[8,3] TAB %brick.eventOutputParameter[8,4]);
	commandToClient(%client,'receiveNPCData',%brick,"HEADN",%brick.eventOutputParameter[9,1] TAB %brick.eventOutputParameter[9,2] TAB %brick.eventOutputParameter[9,3]);
	commandToClient(%client,'receiveNPCData',%brick,"FEETC",%brick.eventOutputParameter[10,1] TAB %brick.eventOutputParameter[10,2] TAB %brick.eventOutputParameter[10,3] TAB %brick.eventOutputParameter[10,4]);
	commandToClient(%client,'receiveNPCData',%brick,"BODYC",%brick.eventOutputParameter[11,1] TAB %brick.eventOutputParameter[11,2] TAB %brick.eventOutputParameter[11,3] TAB %brick.eventOutputParameter[11,4]);
	commandToClient(%client,'receiveNPCData',%brick,"ARMC",%brick.eventOutputParameter[12,1] TAB %brick.eventOutputParameter[12,2] TAB %brick.eventOutputParameter[12,3] TAB %brick.eventOutputParameter[12,4]);
	commandToClient(%client,'receiveNPCData',%brick,"HEADC",%brick.eventOutputParameter[13,1] TAB %brick.eventOutputParameter[13,2] TAB %brick.eventOutputParameter[13,3]);
	commandToClient(%client,'receiveNPCData',%brick,"DECALS",%brick.eventOutputParameter[14,1] TAB %brick.eventOutputParameter[14,2]);
	for(%i=15;%i<%brick.numEvents;%i++)
	{
		if(%brick.eventOutputIdx[%i] == $City::Event::setPose&&InputEvent_getTargetClass("fxDTSBrick",%brick.eventInputIdx[%i],%brick.eventTargetIdx[%i]) $= "NPCSpawn")
		{
			commandToClient(%client,'receiveNPCData',%brick,"POSE",%brick.eventOutputParameter[%i,1] TAB %brick.eventOutputParameter[%i,2] TAB %brick.eventOutputParameter[%i,3] TAB %brick.eventOutputParameter[%i,4]);
			%dataCount++;
		}
	}
	commandToClient(%client,'receiveNPCData',%brick,"FINISH",%dataCount+15);
}

//Request that we send the client a few frequently updated fields from the specified brick.
//Timeout: 1000 ms
//This request is sent when the client clicks the "Reload" button on their GUI.
function serverCmdRequestNPCDataRefresh(%client,%brick)
{
	if(!%client.isAdmin&&!%client.isStaff)
		return;
	if(getSimTime()-%client.NPCSpawnDataTimeout<1000)
		return;
	if(!isObject(%brick))
		return;
	if(%brick.getClassName()!$="fxDTSBrick")
		return;
	if(%brick.getDatablock().getName()!$="brickNPCSpawnData")
		return;

	%client.NPCSpawnDataTimeOut = getSimTime();

	commandToClient(%client,'receiveNPCData',%brick,"MONEY",%brick.eventOutputParameter[2,1]);
	commandToClient(%client,'receiveNPCData',%brick,"HUNGER",%brick.eventOutputParameter[3,1]);
	commandToClient(%client,'receiveNPCData',%brick,"GREED",%brick.eventOutputParameter[4,1] TAB ((%brick.eventOutputParameter[4,2]>0)?strreplace($CityRPG::prices::weapon::name[%brick.eventOutputParameter[4,2]-1].uiName, " ", ""):"NONE") TAB %brick.eventOutputParameter[4,1] TAB %brick.eventOutputParameter[4,2] TAB %brick.eventOutputParameter[4,3] TAB %brick.eventOutputParameter[4,4]);
	commandToClient(%client,'receiveNPCData',%brick,"FINISH",3);
}

//Request that we send the client information regarding a "Rent" brick in front of their player.
//Timeout: 500 ms
//This request is sent when the client clicks the "Get Rent" button on their GUI.
//It is meant to be used to retrieve a raycast which will find a brick that has a "RequestFunds" event on it.
function serverCmdRequestFillRent(%client,%brick)
{
	if(!%client.isAdmin&&!%client.isStaff)
		return;
	if(getSimTime()-%client.NPCSpawnDataTimeout<500)
		return;
	if(!isObject(%brick))
		return;

	if(!isObject(%player = %client.player))
		return;

	%client.NPCSpawnDataTimeOut = getSimTime();

	%cast = containerRayCast(%player.getEyePoint(),VectorAdd(%player.getEyePoint(),vectorScale((%aimVector = %player.getEyeVector()),5)),$TypeMasks::FxBrickAlwaysObjectType);
	if(!isObject(%target = getWord(%cast,0)))
	{
		commandToClient(%client,'NPCEditorDisplayMsg',"No brick.  Look at a brick with a RequestFunds event on it.");
		return;
	}

	%rentEventIDx = outputEvent_GetOutputEventIDx("fxDTSBrick","requestFunds");
	%count = %target.numEvents;
	for(%i=0;%i<%count;%i++)
	{
		if(%target.eventOutputIDx[%i]==%rentEventIDx && InputEvent_getTargetClass("fxDTSBrick",%target.eventInputIdx[%i],%target.eventTargetIdx[%i]) $= "fxDTSBrick")//We MUST check the target class.  Else this could be something other than a requestFunds event that uses the same outputEventIDx, just on another target.  Like a player or a vehicle.
		{
			%eventNum = %i;
			%price = %target.eventOutputParameter[%i,2];
			break;
		}
	}
	if(%price$="")
	{
		commandToClient(%client,'NPCEditorDisplayMsg',"Event 'requestFunds' not found.");
		return;
	}

	%pointa = VectorAdd(getWords(%cast,1,3),vectorScale(%aimVector,-0.25));
	%pointb = VectorAdd(getWords(%cast,1,3),vectorScale(%aimVector,0.25));
	%castAgain = containerRayCast(%pointa,%pointb,$TypeMasks::FxBrickAlwaysObjectType);
	if(getWord(%cast,0) != %target)
	{
		commandToClient(%client,'NPCEditorDisplayMsg',"Non-reproducable aim.  Move and try again?");
		return;
	}

	commandToClient(%client,'receiveNPCData',%brick,"RENT",%price TAB %pointa SPC %pointb SPC %eventNum);
	commandToClient(%client,'receiveNPCData',%brick,"FINISH",1);
}

//Request the client's player position
//Timeout: 500 ms
//This request is sent when the client clicks a "<" button in the "Position" column of the pose editor.
//It is meant to be used to retrieve the player's position.
function serverCmdRequestNPCPlayerPos(%client,%brick,%num)
{
	if(!%client.isAdmin&&!%client.isStaff)
		return;
	if(getSimTime()-%client.NPCSpawnDataTimeout<500)
		return;
	if(!isObject(%brick))
		return;
	if(!isObject(%player = %client.player))
		return;

	%client.NPCSpawnDataTimeOut = getSimTime();
	commandToClient(%client,'receiveNPCData',%brick,"PLAYERPOS",getwords(%player.getTransform(),0,2) TAB %num);
	commandToClient(%client,'receiveNPCData',%brick,"FINISH",1);
}

//Request the client's player rotation
//Timeout: 500 ms
//This request is sent when the client clicks a "<" button in the "Rotation" column of the pose editor.
//It is meant to be used to retrieve the player's rotation.
function serverCmdRequestNPCPlayerRot(%client,%brick,%num)
{
	if(!%client.isAdmin&&!%client.isStaff)
		return;
	if(getSimTime()-%client.NPCSpawnDataTimeout<500)
		return;
	if(!isObject(%brick))
		return;
	if(!isObject(%player = %client.player))
		return;

	%client.NPCSpawnDataTimeOut = getSimTime();
	commandToClient(%client,'receiveNPCData',%brick,"PLAYERROT",getwords(%player.getTransform(),3,6) TAB %num);
	commandToClient(%client,'receiveNPCData',%brick,"FINISH",1);
}

//Set up the client's wrench brick for applying NPC Data
//Timeout: 0 ms
//This request is sent when the client clicks the "Apply" button on their GUI.
//It is meant to set up the wrenchBrick flag, an to temporarily modify trust preferences to allow editing of the specified brick.
function serverCmdNPCsetWrenchBrick(%client,%brick)
{
	%client.wrenchBrick = "";
	if(!%client.isAdmin&&!%client.isStaff)
		return;
	if(!isObject(%brickgroup = %client.brickGroup))
		return;
	if(!isObject(%brick))
		return;
	if(%brick.getClassName()!$="fxDTSBrick")
		return;
	if(%brick.getDatablock().getName()!$="brickNPCSpawnData")
		return;
	if(!isObject(%bricksgroup = %brick.getgroup()))// ;)
		return;
	if(%brickgroup==%bricksgroup)
	{
		%client.wrenchBrick = %brick;
		return;
	}

	%brick.oldTrustLevel = %bricksgroup.Trust[%brickgroup.bl_id];
	%bricksgroup.Trust[%brickgroup.bl_id] = $TrustLevel::WrenchEvents;
	%brickgroup.Trust[%bricksgroup.bl_id] = $TrustLevel::WrenchEvents;

	%client.wrenchBrick = %brick;
}

//Clear the client's wrench brick after applying NPC Data
//Timeout: 0 ms
//This request is sent after a client sends a selection of 'addEvent' commands.
//It is meant to clear the temporary trust modifications, and set the wrenchBrick flag to nothing.
function serverCmdNPCclearWrenchBrick(%client, %brick)
{
	if(!%client.isAdmin&&!%client.isStaff)
		return;
	if(!isObject(%brickgroup = %client.brickGroup))
		return;
	if(!isObject(%brick))
		return;
	if(%brick.getClassName()!$="fxDTSBrick")
		return;
	if(%brick.getDatablock().getName()!$="brickNPCSpawnData")
		return;
	if(!isObject(%bricksgroup = %brick.getgroup()))// ;)
		return;
	if(%brickgroup==%bricksgroup)
		return;

	%bricksgroup.Trust[%brickgroup.bl_id] = %brick.oldTrustLevel;
	%brickgroup.Trust[%bricksgroup.bl_id] = %brick.oldTrustLevel;

	%client.wrenchBrick = "";
}

//Food buying
//	NPCs will never go over 52
//NPCs will never buy over their current money.
//NPCs do NOT starve. :\
//NPCs will buy when their hunger goes below 5.

function City_NPCTickLoop(%loop)
{
	if(!isObject(%fullList = NPCProcessList))
		return;
	if(!isObject(%list = NPCProcessList_Tick))
		return;

	if(!%loop)
	{
		cancel(%list.loopSched);
		%list.clear();
		%count = %fullList.getCount();
		for(%i=0;%i<%count;%i++)
		{
			%list.add(%fullList.getObject(%i));
		}
		%list.time = mFloor(($Pref::Server::City::tick::speed * 60000)/%count);
	}

	if(%loop>=%list.getCount())
		return;
	if(!isObject(%npc = %list.getObject(%loop)))
		return;

	//NPCTempProcess(%npc);

	//Process Money IN
	%money = %npc.eventOutputParameter[2,1];
	
	if(isObject(JobSO.job[%npc.eventOutputParameter[1,1]+1]))
	{
		%pay = JobSO.job[%npc.eventOutputParameter[1,1]+1].pay;
		%ecoMod = $Economics::Condition/100;
		%sum = mFloor((%pay * %ecoMod) + %pay);

		%money += %sum;
		%npc.eventOutputParameter[2,1] = %money;
	}

	//Process hunger DOWN
	%hunger = %npc.eventOutputParameter[3,1];
	%hunger -=1;
	if(%hunger<=0)
		%hunger = 0;
	%npc.eventOutputParameter[3,1] = %hunger;

	//Buy food?
	if(%hunger<5)
	{
		//Pick a shop
		%count = CityRPGEventTracker.sellFood.getCount();
		if(%count>0)
		{
			%shop = CityRPGEventTracker.sellFood.getObject(getRandom(0,%count-1));
			//Pick a product brick
			%productCount = %shop.getCount();
			if(%productCount>0)
			{
				%productBrick = %shop.getObject(getRandom(0,%productCount-1));
				//Find the product
				for(%i=0;%i<%productBrick.numEvents;%i++)
				{
					if(%productBrick.eventOutputIdx[%i] == $City::Event::sellFood && InputEvent_getTargetClass("fxDTSBrick",%productBrick.eventInputIdx[%i],%productBrick.eventTargetIdx[%i]) $= "fxDTSBrick")
					{
						%portion = %productBrick.eventOutputParameter[%i,1];
						%product = %productBrick.eventOutputParameter[%i,2];
						%price = (5 * %portion - mFloor(%portion * 0.75)) + %productBrick.eventOutputParameter[%i,3];
						%profit = %productBrick.eventOutputParameter[%i,3];
						break;
					}
				}
				if(%price <= %money && %profit <= mCeil(%price/2))//Don't let them charge over %200.
				{
					%sellerBG = %productBrick.getGroup();
					%sellerID = %sellerBG.bl_id;
					if(isObject(%sellerData = CityRPGData.getData(%sellerID)))
					{
						if(JobSO.job[%sellerData.valueJobID].sellFood)
						{
							%sellerData.valueBank += %profit;
							%sellerBG.foodIncome += %profit;
							%sellerBG.foodNumPortions += %portion;

							%money -= %price;
							%npc.eventOutputParameter[2,1] = %money;

							%hunger += %portion;
							%npc.eventOutputParameter[3,1] = %hunger;
						}
					}
				}
			}
		}
	}

	//Pay rent?
	//We pay rent on the 1st of each month.
	%so = CalendarSO;
	%day = %so.date;
	for(%a = 0; %day > %so.daysInMonth[%a % %so.numOfMonths]; %a++)
	{
		%day -= %so.daysInMonth[%a % %so.numOfMonths];
	}
	if(%day == 1)
	{
		//Try to pay rent.

		//Avoid word count errors :P
		if(getWordCount(%npc.eventOutputParameter[5,2]) == 7)
		{
			//Find Brick
			%pointA = getWords(%npc.eventOutputParameter[5,2],0,2);
			%pointB = getWords(%npc.eventOutputParameter[5,2],3,5);
			%cast = containerRayCast(%pointA,%pointB,$TypeMasks::FxBrickAlwaysObjectType);
			%rentBrick = getWord(%cast,0);
			if(isObject(%rentBrick))
			{
				//Can this guy sell services?
				%landLordBG = %rentBrick.getGroup();
				%landLordID = %landLordBG.bl_id;
				if(isObject(%landLordData = CityRPGData.getData(%landLordID)))
				{
					if(JobSO.job[%landLordData.valueJobID].sellServices)
					{
						//Find event
						%eventNum = getWord(%npc.eventOutputParameter[5,2],6);
						%rentEventIDx = outputEvent_GetOutputEventIDx("fxDTSBrick","requestFunds");
						if(%rentBrick.eventOutputIDx[%eventNum]==%rentEventIDx && InputEvent_getTargetClass("fxDTSBrick",%rentBrick.eventInputIdx[%eventNum],%rentBrick.eventTargetIdx[%eventNum]) $= "fxDTSBrick")
						{
							//Check Price
							echo("Found rent for "@%so.nameOfMonth[%so.getMonth()]@" of $"@%rentBrick.eventOutputParameter[%eventNum,2]);

							%budget = %npc.eventOutputParameter[5,1];
							if((%rentPrice = %rentBrick.eventOutputParameter[%eventNum,2])>%budget)
								%rentPrice = %budget;
							if(%rentPrice > %money)
								%rentPrice = %money;

							%money -= %rentPrice;
							%npc.eventOutputParameter[2,1] = %money;

							%landLordData.valueBank += %rentPrice;
							%landLordBG.rentIncome += %rentPrice;
							%landLordBG.rentNumPayments += 1;
						}
					}
				}
			}
		}
	}

	//Buy greed
	%rerollGreed = true;
	if(isObject(%itemBrick = %npc.eventOutputParameter[4,3]))
	{
		if(%itemBrick.getClassName()$="fxDTSBrick")
		{
			%armsDealerBG = %itemBrick.getGroup();
			%armsDealerID = %armsDealerBG.bl_id;
			if(isObject(%armsDealerData = CityRPGData.getData(%armsDealerID)))
			{
				if(JobSO.job[%armsDealerData.valueJobID].sellItems)
				{
					%eventNum = %npc.eventOutputParameter[4,4];
					if(%itemBrick.eventOutputIdx[%eventNum] == $City::Event::sellItem && InputEvent_getTargetClass("fxDTSBrick",%itemBrick.eventInputIdx[%eventNum],%itemBrick.eventTargetIdx[%eventNum]) $= "fxDTSBrick")
					{
						if(%npc.eventOutputParameter[4,2]-1 == (%itemListIDx = %itemBrick.eventOutputParameter[%eventNum,1]))
						{
							if(%npc.eventOutputParameter[4,1] == ((%itemPrice = $CityRPG::prices::weapon::price[%itemListIDx])+(%itemProfit = %itemBrick.eventOutputParameter[%eventNum,2])) && $CityRPG::prices::weapon::mineral[%itemListIDx] <= CitySO.minerals)
							{
								if(%npc.eventOutputParameter[5,1] + %itemPrice + %itemProfit <= %money)
								{

									%money -= (%itemPrice + %itemProfit);
									%npc.eventOutputParameter[2,1] = %money;

									CitySO.minerals -= $CityRPG::prices::weapon::mineral[%itemListIDx];

									%armsDealerData.valueBank += %itemProfit;
									%armsDealerBG.itemIncome += %itemProfit;
									%armsDealerBG.itemNumSales += 1;
								}
								else
								{
									%rerollGreed = false;
								}
							}
						}
					}
				}
			}
		}
	}

	//Reroll Greed
	if(%rerollGreed)
	{
		//Pick a shop
		%count = CityRPGEventTracker.sellItem.getCount();
		if(%count>0)
		{
			%shop = CityRPGEventTracker.sellItem.getObject(getRandom(0,%count-1));
			//Pick a product brick
			%productCount = %shop.getCount();
			if(%productCount>0)
			{
				%productBrick = %shop.getObject(getRandom(0,%productCount-1));
				%productBrickData = CityRPGData.getData(%productBrick.getGroup().bl_id);
				if(isObject(%productBrickData))
				{
					if(JobSO.job[%productBrickData.valueJobID].sellItems)
					{
						//Find the product
						for(%i=0;%i<%productBrick.numEvents;%i++)
						{
							if(%productBrick.eventOutputIdx[%i] == $City::Event::sellItem && InputEvent_getTargetClass("fxDTSBrick",%productBrick.eventInputIdx[%i],%productBrick.eventTargetIdx[%i]) $= "fxDTSBrick")
							{
								%rerollItemIdx = %productBrick.eventOutputParameter[%i,1];
								%rerollProfit = %productBrick.eventOutputParameter[%i,2];
								%rerollPrice = $CityRPG::prices::weapon::price[%rerollItemIdx];
								break;
							}
						}
						if((%rerollPrice + %rerollProfit) <= (JobSO.job[%npc.eventOutputParameter[1,1]+1].pay * 20))
						{
							%npc.eventOutputParameter[4,1] = (%rerollPrice + %rerollProfit);
							%npc.eventOutputParameter[4,2] = %rerollItemIdx+1;
							%npc.eventOutputParameter[4,3] = %productBrick;
							%npc.eventOutputParameter[4,4] = %i;
						}
					}
				}
			}
		}
	}

	%list.loopSched = schedule(%list.time, %list, "City_NPCTickLoop", (%loop + 1));
}

function City_NPCPoseLoop(%loop)
{
	%time = (($Pref::Server::City::tick::speed * 60000)/24);

	if(!isObject(%avg = NPCAvatarGroup))
		return;
	%hour = (%loop+6)%24;
	if(!isObject(%hourSet = %avg.hour[%hour]))//The tick rolls over at 6 AM.  So we need to run @ + 6 hours here
		return;

	%count = %hourSet.getCount();
	for(%i=0;%i<%count;%i++)
	{
		%avatar = %hourSet.getObject(%i);
		%countP = %avatar.posecount[%hour];
		for(%p=0;%p<%countP;%p++)
		{
			%pose = %avatar.pose[%hour,%p];
			schedule(mFloatLength(%time*getField(%pose,0),0),%avatar,"City_DoAvatarPose",%avatar,getField(%pose,1),(getFieldCount(%pose)>=3?getField(%pose,2):""));
		}
	}

	schedule(%time, %avg, "City_NPCPoseLoop", (%loop + 1));
}

function City_DoAvatarPose(%avatar,%transform,%animation)
{
	%avatar.setTransform(%transform);
	if(%animation!$="")
	{
		%avatar.stopThread(0);
		%avatar.playThread(0,%animation);
	}
}

deactivatepackage(NPCPackage);
package NPCPackage
{
	function onMissionLoaded()
	{
		Parent::onMissionLoaded();
		%avg = new SimGroup("NPCAvatarGroup");
		MissionCleanup.add(NPCAvatarGroup);
		for(%i=0;%i<24;%i++)
		{
			%avg.hour[%i] = new SimSet();
			MissionCleanup.add(%avg.hour[%i]);
		}
	}
};
activatepackage(NPCPackage);


if($CityRPG::portion[7] !$= ""||$CityRPG::portion[6] $= "")//Did they add or subtract the amt of portions?
	warn(expandFileName("./npcs.cs")@" - Food portion sizes changed!  NPC module may need some prices changed!");
