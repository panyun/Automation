using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component.DTO
{
	public class SelectVariableRequest : CommponetRequest
	{
		public SelectVariableRequest()
		{
			ComponentName = nameof(SelectVariableRequest);
		}
		public List<Type> Types { get; set; }
	}
}
