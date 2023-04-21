#pragma once 
#include "CoreMinimal.h" 
namespace Net {
	enum CMD
	 {
		   C2R_Login = 61001,
		   R2C_Login = 61002,
		   C2G_Enter = 61003,
		   G2C_Enter = 61004,
		   C2G_Ping = 61005,
		   G2C_Ping = 61006,
		   G2C_Abandoned = 61007,
		   C2G_MsgAgent = 61008,
		   G2C_MsgAgent = 61009,
		   C2G_FileAgent = 61010,
		   G2C_FileAgent = 61011,
		   G2C_OnLine = 61012,
		   G2C_OffLine = 61013,
		   C2M_TestRobotCase = 61014,
		   M2C_TestRobotCase = 61015,
	};
}
