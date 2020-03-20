using System;
using Microsoft.Extensions.Configuration;

namespace EntityGenerator {

	/// <summary>
	/// Summary description for Class1
	/// </summary>
	public class AppSettings {

		public static IConfiguration Configuration { get; set; }

		public class MySqlSettings {
			public static string Server { get { return Configuration["MySqlSettings:Server"]; } }
			public static string Database { get { return Configuration["MySqlSettings:Database"]; } }
			public static string User { get { return Configuration["MySqlSettings:User"]; } }
			public static string Password { get { return Configuration["MySqlSettings:Password"]; } }
			public static string Connection { get { return Configuration["MySqlSettings:Connection"]; } }
		}

	}
}
