using EntityGenerator.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntityGenerator.Util;

namespace EntityGenerator.Controller {

	public class EntityGenCSharpController {

		/// <summary>
		/// データ型変換テーブル
		/// </summary>
		private static Dictionary<string, string> ConvertTable = new Dictionary<string, string>() {
			{"System.Byte", "byte" },
			{"System.SByte", "byte" },
			{"System.Int32", "int"},
			{"System.UInt32", "uint"},
			{"System.String", "string"},
			{"System.DateTime","DateTime"}
		};

		private static string EntiryFormat = "	public {0} {1} {{ get; set; }}\n";

		/// <summary>
		/// テーブル構造情報からエンティティクラスコードを作成する
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public Task<EntityGenCSharpViewData> GenerateCode(EntityGenCSharpViewData data) {

			//Debug.Log(System.IO.Directory.GetCurrentDirectory());
			if (string.IsNullOrEmpty(data.TableName)) {
				return Task.FromResult(data);
			}

			//テンプレートパス指定
			string path = FileHandler.PathCombine("Template", "EntityTemplate.txt");

			string template = FileHandler.ReadFile(path);
			template = template.Replace("_____CLASS_____", ToUpperCamelTableName(data.TableName));
			template = template.Replace("_____MEMBERS_____", GetEntityString(data.TableName));
			data.OutPut = template;

			return Task.FromResult(data);
		}

		/// <summary>
		/// コード生成
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static string GetEntityString(string tableName) {

			string result = string.Empty;
			// MySQLへの接続
			try {
				string connectionStr = string.Format(
					AppSettings.MySqlSettings.Connection,
					AppSettings.MySqlSettings.Server,
					AppSettings.MySqlSettings.Database,
					AppSettings.MySqlSettings.User,
					AppSettings.MySqlSettings.Password
				);

				MySqlConnection connection = new MySqlConnection(connectionStr);
				connection.Open();

				//SQL実行
				MySqlCommand cmd = new MySqlCommand(string.Format("SELECT * FROM {0} LIMIT 1", tableName), connection);
				MySqlDataReader reader = cmd.ExecuteReader();

				//カラム名出力
				StringBuilder builder = new StringBuilder();
				string[] names = new string[reader.FieldCount];
				string[] types = new string[reader.FieldCount];
				for (int i = 0; i < reader.FieldCount; i++) {
					string colmun = ToUpperCamel(reader.GetName(i));
					string str = string.Format(EntiryFormat, ConvertTable[reader.GetFieldType(reader.GetName(i)).ToString()], colmun);
					builder.Append(str);
				}

				result = builder.ToString();

				// 接続の解除
				connection.Close();
			} catch (MySqlException me) {
				Console.WriteLine("ERROR: " + me.Message);
			}

			return result;

		}

		/// <summary>
		/// キャメルへ変換
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string ToCamel(string text) {
			return Regex.Replace(
				text.ToLower(),
				@"_\w",
				match => match.Value.ToUpper().Replace("_", string.Empty));
		}

		/// <summary>
		/// アッパーキャラメル変換
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string ToUpperCamel(string text) {
			text = ToCamel(text);
			return Regex.Replace(
				text,
				@"^\w",
				match => match.Value.ToUpper());
		}

		/// <summary>
		/// テーブル名をアッパーキャラメルに変換する。
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <returns>変換後のテーブル名</returns>
		public static string ToUpperCamelTableName(string tableName) {
			if (tableName.ToUpper().StartsWith("D_") ||
				tableName.ToUpper().StartsWith("M_") ||
				tableName.ToUpper().StartsWith("T_") ||
				tableName.ToUpper().StartsWith("V_")) {
				// 先頭が「D_」「M_」「T_」「V_」の場合は除去する
				tableName = tableName.Substring(2);
			}
			// パスカルケースに変換する
			return ToUpperCamel(tableName);
		}
	}

}