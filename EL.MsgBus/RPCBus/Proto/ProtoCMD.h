#pragma once 
#include "CoreMinimal.h" 
namespace Net {
	enum CMD
	 {
		   C2R_Login = 20025,
		   R2C_Login = 20026,
		   C2G_Enter = 20027,
		   G2C_Enter = 20028,
		   C2G_Ping = 20029,
		   G2C_Ping = 20030,
		   G2C_Abandoned = 20031,
		   P2C_CreatePlayer = 20032,
		   C2P_TestActor = 20033,
		   P2C_TestActor = 20034,
		   C2P_FormationCreate = 20035,
		   P2C_FormationCreate = 20036,
		   C2P_FormationAddShip = 20037,
		   P2C_FormationAddShip = 20038,
		   C2P_FormationRemoveShip = 20039,
		   P2C_FormationRemoveShip = 20040,
		   C2P_FormationDisband = 20041,
		   P2C_FormationDisband = 20042,
		   C2P_FormationRename = 20043,
		   P2C_FormationRename = 20044,
		   C2P_FormationGetEventLog = 20045,
		   P2C_FormationGetEventLog = 20046,
		   P2C_FormationGetNewEventLog = 20047,
		   RoleInfo = 20048,
		   Tile = 20049,
		   Position = 20050,
		   Formation = 20051,
		   Ship = 20052,
		   Equip = 20053,
		   EventLog = 20054,
	};
}
