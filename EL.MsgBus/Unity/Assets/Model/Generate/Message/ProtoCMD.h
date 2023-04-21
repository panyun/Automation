#pragma once 
#include "CoreMinimal.h" 
namespace Net {
	enum CMD
	 {
		   C2M_TestRequest = 20001,
		   M2C_TestResponse = 20002,
		   Actor_TransferRequest = 20003,
		   Actor_TransferResponse = 20004,
		   C2G_EnterMap = 20005,
		   G2C_EnterMap = 20006,
		   UnitInfo = 20007,
		   M2C_CreateUnits = 20008,
		   C2M_PathfindingResult = 20009,
		   C2M_Stop = 20010,
		   M2C_PathfindingResult = 20011,
		   M2C_Stop = 20012,
		   C2G_Ping = 20013,
		   G2C_Ping = 20014,
		   G2C_Test = 20015,
		   C2M_Reload = 20016,
		   M2C_Reload = 20017,
		   C2R_Login = 20018,
		   R2C_Login = 20019,
		   C2G_LoginGate = 20020,
		   G2C_LoginGate = 20021,
		   G2C_TestHotfixMessage = 20022,
		   C2M_TestRobotCase = 20023,
		   M2C_TestRobotCase = 20024,
	};
}
