using System;
using System.IO;
using System.Text;

namespace EntityGenerator.Util {

	public class FileHandler {

		public static string PathCombine(params string[] paths) {
			return Path.Combine(paths);
		}

		/// <summary>
		/// ファイルを読み込んで文字列を取得する
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string ReadFile(string path) {

			string str = string.Empty;

			if (File.Exists(path)) {

				StreamReader sr = new StreamReader(path, Encoding.GetEncoding("utf-8"));
				str = sr.ReadToEnd();
				sr.Close();

			} else {
				Console.WriteLine("ファイルが存在しません");
			}
			return str;
		}
	}
}
