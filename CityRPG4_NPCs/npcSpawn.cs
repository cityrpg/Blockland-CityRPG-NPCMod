datablock fxDTSBrickData(brickNPCSpawnData)
{
	brickFile = $CityNPCs::DataPath @ "NPCSpawn.blb";
	category = "CityRPG";
	subCategory = "Spawns";
	uiName = "NPC Spawn";
	iconName = $CityNPCs::DataPath @ "NPCSpawn";

	CityRPGBrickAdmin = true;
	CityRPGBrickEventsLocked = true;
};

function brickNPCSpawnData::onRemove(%this, %brick)
{
	Parent::onRemove(%this,%brick);
	if(isObject(%brick))
	{
		if(isObject(%brick.NPCAvatar))
		{
			%brick.NPCAvatar.schedule(0,"delete");
		}
	}
}

function brickNPCSpawnData::onDeath(%this, %brick)
{
	Parent::onDeath(%this,%brick);
	if(isObject(%brick))
	{
		if(isObject(%brick.NPCAvatar))
		{
			%brick.NPCAvatar.schedule(0,"delete");
		}
	}
}

function brickNPCSpawnData::createAvatar(%this, %brick)
{
	if(isObject(%brick.NPCAvatar))
		%brick.NPCAvatar.delete();

	%poseEvent = false;
	for(%i=15;%i<%brick.numEvents;%i++)//If we can't find a setPose event, don't make an avatar.
	{
		if(%brick.eventOutputIdx[%i] == $City::Event::setPose&&InputEvent_getTargetClass("fxDTSBrick",%brick.eventInputIdx[%i],%brick.eventTargetIdx[%i]) $= "NPCSpawn")
		{
			%poseEvent = true;
			break;
		}
	}
	if(!%poseEvent)
		return;

	%brick.NPCAvatar = %avatar = new StaticShape() {
		position = %brick.position;
		rotation = "0 0 1 "@90*((%brick.angleID-1)%4);
		scale = "1 1 1";
		datablock = "SSNPCAvatarData";
		canSetIFLs = true;
	};
	NPCAvatarGroup.add(%avatar);
	HideAllNodes(%avatar);
	%avatar.hideNode("hair");
	%avatar.hideNode("headSkin");

	%db = %avatar.getDatablock();
	if(%db.hip[%brick.eventOutputParameter[7,1]] !$= "SkirtHip")
	{
		if((%node = %db.lleg[%brick.eventOutputParameter[6,1]]) !$= "NONE")
		{
			%avatar.unHideNode(%node);
			%avatar.setNodeColor(%node,%brick.eventOutputParameter[10,1]);
		}
		if((%node = %db.rleg[%brick.eventOutputParameter[6,2]]) !$= "NONE")
		{
			%avatar.unHideNode(%node);
			%avatar.setNodeColor(%node,%brick.eventOutputParameter[10,2]);
		}
	}
	if((%node = %db.lski[%brick.eventOutputParameter[6,3]]) !$= "NONE")
	{
		%avatar.unHideNode(%node);
		%avatar.setNodeColor(%node,%brick.eventOutputParameter[10,3]);
	}
	if((%node = %db.rski[%brick.eventOutputParameter[6,4]]) !$= "NONE")
	{
		%avatar.unHideNode(%node);
		%avatar.setNodeColor(%node,%brick.eventOutputParameter[10,4]);
	}
	if((%node = %db.hip[%brick.eventOutputParameter[7,1]]) !$= "NONE")
	{
		%avatar.unHideNode(%node);
		%avatar.setNodeColor(%node,%brick.eventOutputParameter[11,1]);
		if(%node $= "SkirtHip")
		{
			%avatar.unHideNode("SkirtTrimLeft");
			%avatar.setNodeColor("SkirtTrimLeft",%brick.eventOutputParameter[10,1]);
			%avatar.unHideNode("SkirtTrimRight");
			%avatar.setNodeColor("SkirtTrimRight",%brick.eventOutputParameter[10,2]);
		}
	}
	if((%node = %db.chest[%brick.eventOutputParameter[7,2]]) !$= "NONE")
	{
		%avatar.unHideNode(%node);
		%avatar.setNodeColor(%node,%brick.eventOutputParameter[11,2]);
	}
	if((%node = %db.pack[%brick.eventOutputParameter[7,3]]) !$= "NONE")
	{
		%avatar.unHideNode(%node);
		%avatar.setNodeColor(%node,%brick.eventOutputParameter[11,3]);
		%headup = true;
	}
	if((%node = %db.secondpack[%brick.eventOutputParameter[7,4]]) !$= "NONE")
	{
		%avatar.unHideNode(%node);
		%avatar.setNodeColor(%node,%brick.eventOutputParameter[11,4]);
		%headup = true;
	}
	if((%node = %db.larm[%brick.eventOutputParameter[8,1]]) !$= "NONE")
	{
		%avatar.unHideNode(%node);
		%avatar.setNodeColor(%node,%brick.eventOutputParameter[12,1]);
	}
	if((%node = %db.rarm[%brick.eventOutputParameter[8,2]]) !$= "NONE")
	{
		%avatar.unHideNode(%node);
		%avatar.setNodeColor(%node,%brick.eventOutputParameter[12,2]);
	}
	if((%node = %db.lhand[%brick.eventOutputParameter[8,3]]) !$= "NONE")
	{
		%avatar.unHideNode(%node);
		%avatar.setNodeColor(%node,%brick.eventOutputParameter[12,3]);
	}
	if((%node = %db.rhand[%brick.eventOutputParameter[8,4]]) !$= "NONE")
	{
		%avatar.unHideNode(%node);
		%avatar.setNodeColor(%node,%brick.eventOutputParameter[12,4]);
	}
	if((%node = %db.head[%brick.eventOutputParameter[9,1]]) !$= "NONE")
	{
		%avatar.unHideNode(%node);
		%avatar.setNodeColor(%node,%brick.eventOutputParameter[13,1]);
	}
	if((%node = %db.hat[%brick.eventOutputParameter[9,2]]) !$= "NONE")
	{
		%avatar.unHideNode(%node);
		%avatar.setNodeColor(%node,%brick.eventOutputParameter[13,2]);
	}
	if((%node = %db.accent[%brick.eventOutputParameter[9,3]]) !$= "NONE")
	{
		%avatar.unHideNode(%node);
		%avatar.setNodeColor(%node,%brick.eventOutputParameter[13,3]);
	}

	%avatar.setIflFrame("face",%brick.eventOutputParameter[14,1]);
	%avatar.setIflFrame("decal",%brick.eventOutputParameter[14,2]);

	if(%headup)
		%avatar.playThread(3,"headUp");

	//Add pose data
	for(%i=15;%i<%brick.numEvents;%i++)
	{
		if(%brick.eventInputIDx[%i] == $City::Event::setNPCData && %brick.eventTargetIDx[%i] == $City::Event::setNPCData["And"] && %brick.eventOutputIDx[%i] == $City::Event::setPose)
		{
			%hour = mAbs(getSubStr(%brick.eventOutputParameter[%i,1],0,2));
			%minute = mAbs(getSubStr(%brick.eventOutputParameter[%i,1],3,2));
			%avatar.pose[%hour,mAbs(%avatar.posecount[%hour])] = %minute/60 TAB %brick.eventOutputParameter[%i,2] @ (%brick.eventOutputParameter[%i,3]?"\t"@%brick.eventOutputParameter[%i,4]:"");
			%avatar.posecount[%hour]++;
			NPCAvatarGroup.hour[%hour].add(%avatar);
		}
	}
	//Why am I doing this?  Shouldn't I be using the event data from the brick?  It's hardly different compared to storing it on the avatar.
	//Maybe instead of storing the complete pose data, all we need to store is which event the data is stored in.
	//...
	//... this is all so dumb and i hate it >_<
}

function brickNPCSpawnData::onPlant(%this, %brick)
{
	Parent::onPlant(%this,%brick);//we probably dont need this

	//Should we be using %brick.addEvent(%enabled,%delay,%input,%target,%output,%par1,%par2,%par3,%par4); ??
	//I just copied what one of the default add-ons did.  But like- we're basically just duplicating the functionality of fxDTSBrick::addEvent() for no reason.
	%inputIdx = inputEvent_GetInputEventIdx("setNPCData");
	%i=0;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setNPCName";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setNPCName");
	%brick.eventOutputParameter[%i,1] = "John Doe";
	%brick.eventOutputParameter[%i,2] = "";
	%brick.eventOutputParameter[%i,3] = "";
	%brick.eventOutputParameter[%i,4] = "";
	%i++;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setJob";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setJob");
	%brick.eventOutputParameter[%i,1] = "0";
	%brick.eventOutputParameter[%i,2] = "";
	%brick.eventOutputParameter[%i,3] = "";
	%brick.eventOutputParameter[%i,4] = "";
	%i++;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setMoney";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setMoney");
	%brick.eventOutputParameter[%i,1] = "250";
	%brick.eventOutputParameter[%i,2] = "";
	%brick.eventOutputParameter[%i,3] = "";
	%brick.eventOutputParameter[%i,4] = "";
	%i++;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setHunger";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setHunger");
	%brick.eventOutputParameter[%i,1] = "6";
	%brick.eventOutputParameter[%i,2] = "";
	%brick.eventOutputParameter[%i,3] = "";
	%brick.eventOutputParameter[%i,4] = "";
	%i++;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setGreed";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setGreed");
	%brick.eventOutputParameter[%i,1] = "0";
	%brick.eventOutputParameter[%i,2] = "0";
	%brick.eventOutputParameter[%i,3] = "";
	%brick.eventOutputParameter[%i,4] = "0";
	%i++;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setMoney";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setRent");
	%brick.eventOutputParameter[%i,1] = "250";
	%brick.eventOutputParameter[%i,2] = "";
	%brick.eventOutputParameter[%i,3] = "";
	%brick.eventOutputParameter[%i,4] = "";
	%i++;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setFootNodes";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setFootNodes");
	%brick.eventOutputParameter[%i,1] = "1";
	%brick.eventOutputParameter[%i,2] = "1";
	%brick.eventOutputParameter[%i,3] = "0";
	%brick.eventOutputParameter[%i,4] = "0";
	%i++;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setBodyNodes";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setBodyNodes");
	%brick.eventOutputParameter[%i,1] = "1";
	%brick.eventOutputParameter[%i,2] = "1";
	%brick.eventOutputParameter[%i,3] = "0";
	%brick.eventOutputParameter[%i,4] = "0";
	%i++;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setArmNodes";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setArmNodes");
	%brick.eventOutputParameter[%i,1] = "1";
	%brick.eventOutputParameter[%i,2] = "1";
	%brick.eventOutputParameter[%i,3] = "1";
	%brick.eventOutputParameter[%i,4] = "1";
	%i++;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setHeadNodes";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setHeadNodes");
	%brick.eventOutputParameter[%i,1] = "1";
	%brick.eventOutputParameter[%i,2] = "0";
	%brick.eventOutputParameter[%i,3] = "0";
	%brick.eventOutputParameter[%i,4] = "";
	%i++;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setFootColors";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setFootColors");
	%brick.eventOutputParameter[%i,1] = "0 0 1 1";
	%brick.eventOutputParameter[%i,2] = "0 0 1 1";
	%brick.eventOutputParameter[%i,3] = "0 0 1 1";
	%brick.eventOutputParameter[%i,4] = "0 0 1 1";
	%i++;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setBodyColors";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setBodyColors");
	%brick.eventOutputParameter[%i,1] = "0 0 1 1";
	%brick.eventOutputParameter[%i,2] = "0.9 0.9 0.9 1";
	%brick.eventOutputParameter[%i,3] = "0 0.435294 0.831373 1";
	%brick.eventOutputParameter[%i,4] = "0 1 0 1";
	%i++;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setArmColors";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setArmColors");
	%brick.eventOutputParameter[%i,1] = "0.9 0 0 1";
	%brick.eventOutputParameter[%i,2] = "0.9 0 0 1";
	%brick.eventOutputParameter[%i,3] = "1 0.878431 0.611765 1";
	%brick.eventOutputParameter[%i,4] = "1 0.878431 0.611765 1";
	%i++;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setHeadColors";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setHeadColors");
	%brick.eventOutputParameter[%i,1] = "1 0.878431 0.611765 1";
	%brick.eventOutputParameter[%i,2] = "1 1 0 1";
	%brick.eventOutputParameter[%i,3] = "0 0.2 0.639216 0.698039";
	%brick.eventOutputParameter[%i,4] = "";
	%i++;
	%brick.eventEnabled[%i] = true;
	%brick.eventDelay[%i] = 0;
	%brick.eventInput[%i] = "setNPCData";
	%brick.eventInputIdx[%i] = %inputIdx;
	%brick.eventTarget[%i] = "And";
	%brick.eventTargetIdx[%i] = 0;
	%brick.eventOutput[%i] = "setDecals";
	%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setDecals");
	%brick.eventOutputParameter[%i,1] = "0";
	%brick.eventOutputParameter[%i,2] = "0";
	%brick.eventOutputParameter[%i,3] = "";
	%brick.eventOutputParameter[%i,4] = "";
	%i++;
	if($Pref::Server::City::NPC::startPose)
	{
		%brick.eventEnabled[%i] = true;
		%brick.eventDelay[%i] = 0;
		%brick.eventInput[%i] = "setNPCData";
		%brick.eventInputIdx[%i] = %inputIdx;
		%brick.eventTarget[%i] = "And";
		%brick.eventTargetIdx[%i] = 0;
		%brick.eventOutput[%i] = "setPose";
		%brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","setPose");
		%brick.eventOutputParameter[%i,1] = "01:00";
		%brick.eventOutputParameter[%i,2] = %brick.position SPC "0 0 1" SPC ($pi/2)*((%brick.angleID-1)%4);
		%brick.eventOutputParameter[%i,3] = "0";
		%brick.eventOutputParameter[%i,4] = "";
		%i++;
	}
	%brick.numEvents = %i;//
	// %brick.eventEnabled[%i] = true;
	// %brick.eventDelay[%i] = 0;
	// %brick.eventInput[%i] = "setNPCData";
	// %brick.eventInputIdx[%i] = %inputIdx;
	// %brick.eventTarget[%i] = "And";
	// %brick.eventTargetIdx[%i] = 0;
	// %brick.eventOutput[%i] = "finish";
	// %brick.eventOutputIdx[%i] = outputEvent_GetOutputEventIdx("NPCSpawn","finish");
	// %brick.eventOutputParameter[%i,1] = "";
	// %brick.eventOutputParameter[%i,2] = "";
	// %brick.eventOutputParameter[%i,3] = "";
	// %brick.eventOutputParameter[%i,4] = "";
	// %i++;
	// %brick.numEvents = %i;
	%brick.addEvent(1,0,"setNPCData","And","Finish","","","","");

	if(isObject(NPCSpawnList))
		NPCSpawnList.add(%brick);
}

function brickNPCSpawnData::onLoadPlant(%this,%brick)
{
	Parent::onLoadPlant(%this,%brick);

	if(isObject(NPCSpawnList))
		NPCSpawnList.add(%brick);
}
