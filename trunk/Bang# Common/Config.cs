// Config.cs
//
// Author:  WOnder93 <omosnacek@gmail.com>
// 
// Copyright (c) 2012 Ondrej Mosnáček
// 
// Created with the help of the source code of KBang (http://code.google.com/p/kbang)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.IO;

namespace Bang
{
	/// <summary>
	/// This class provides access to a simple configuration file.
	/// </summary>
	public class Config
	{
		private static readonly object Lock = new object();
		private static readonly string FileName = Path.Combine(Utils.ConfigFolder, "BangSharp.ini");
		private static Config instance;
		private static readonly string Default = @"
			[Server.Name]
			Bang# Server
			[Server.Description]

			[Server.Port]
			2147
			[Server.AdminPort]
			2148
			[ServerList.Adresses]
			localhost
			wonder93.gotdns.com
			[ServerList.Ports]
			2147
			2147";
		private Dictionary<string, List<string>> values;
		
		private Config(TextReader reader)
		{
			values = new Dictionary<string, List<string>>();
			string lastKey = null;
			while(reader.Peek() >= 0)
			{
				string line = reader.ReadLine().TrimStart(null).TrimEnd(null);
				if(line.StartsWith("[") && line.EndsWith("]"))
				{
					lastKey = line.Substring(1, line.Length - 2);
					values.Add(lastKey, new List<string>());
				}
				else if(line.Length == 0)
					continue;
				else if(lastKey != null)
					values[lastKey].Add(line);
			}
			reader.Close();
		}
		
		static Config()
		{
			try
			{
				instance = new Config(new StreamReader(File.OpenRead(FileName)));
			}
			catch
			{
				instance = new Config(new StringReader(Default));
				instance.Save();
			}
		}
		
		private void Save()
		{
			try
			{
				if(!File.Exists(FileName))
					Directory.CreateDirectory(Utils.ConfigFolder);
				StreamWriter writer = new StreamWriter(File.Create(FileName));
				foreach(string k in values.Keys)
				{
					List<string> list = values[k];
					if(list.Count == 0)
						continue;
					writer.WriteLine('[' + k + ']');
					foreach(string v in list)
						writer.WriteLine(v);
					writer.WriteLine();
				}
				writer.Close();
			}
			catch
			{
			}
		}

		/// <summary>
		/// Gets the instance of this class.
		/// </summary>
		public static Config Instance
		{
			get { return instance; }
		}

		/// <summary>
		/// Gets the <see cref="System.String"/> for the specified key.
		/// </summary>
		/// <param name="key">
		/// The key of the configuration entry.
		/// </param>
		/// <param name="def">
		/// The default value.
		/// </param>
		/// <returns>
		/// The value of the entry with the specified key or <paramref name="def"/> if none is found.
		/// </returns>
		public string GetString(string key, string def)
		{
			try
			{
				List<string> list = values[key];
				if(list.Count == 0)
					return def;
				return list[0];
			}
			catch(KeyNotFoundException)
			{
				return def;
			}
		}
		/// <summary>
		/// Gets the <see cref="System.Int32"/> for the specified key.
		/// </summary>
		/// <param name="key">
		/// The key of the configuration entry.
		/// </param>
		/// <param name="def">
		/// The default value.
		/// </param>
		/// <returns>
		/// The value of the entry with the specified key or <paramref name="def"/> if none is found.
		/// </returns>
		public int GetInteger(string key, int def)
		{
			try
			{
				List<string> list = values[key];
				if(list.Count == 0)
					return def;
				return int.Parse(list[0]);
			}
			catch(FormatException)
			{
				return def;
			}
			catch(KeyNotFoundException)
			{
				return def;
			}
		}
		/// <summary>
		/// Gets the <see cref="System.Bool"/> for the specified key.
		/// </summary>
		/// <param name="key">
		/// The key of the configuration entry.
		/// </param>
		/// <param name="def">
		/// The default value.
		/// </param>
		/// <returns>
		/// The value of the entry with the specified key or <paramref name="def"/> if none is found.
		/// </returns>
		public bool GetBoolean(string key, bool def)
		{
			try
			{
				List<string> list = values[key];
				if(list.Count == 0)
					return def;
				if(list[0] == "true")
					return true;
				else if(list[0] == "false")
					return false;
				else
					return def;
			}
			catch(KeyNotFoundException)
			{
				return def;
			}
		}
		/// <summary>
		/// Gets the list of <see cref="System.String"/> for the specified key.
		/// </summary>
		/// <param name="key">
		/// The key of the configuration entry.
		/// </param>
		/// <param name="def">
		/// The default value.
		/// </param>
		/// <returns>
		/// The value of the entry with the specified key or <paramref name="def"/> if none is found.
		/// </returns>
		public List<string> GetStringList(string key)
		{
			try
			{
				List<string> list = new List<string>(values[key]);
				return list;
			}
			catch(KeyNotFoundException)
			{
				return new List<string>();
			}
		}
		/// <summary>
		/// Gets the list of <see cref="System.Int32"/> for the specified key.
		/// </summary>
		/// <param name="key">
		/// The key of the configuration entry.
		/// </param>
		/// <param name="def">
		/// The default value.
		/// </param>
		/// <returns>
		/// The value of the entry with the specified key or <paramref name="def"/> if none is found.
		/// </returns>
		public List<int> GetIntegerList(string key)
		{
			try
			{
				List<string> list = values[key];
				return list.ConvertAll<int>(int.Parse);
			}
			catch(FormatException)
			{
				return new List<int>();
			}
			catch(KeyNotFoundException)
			{
				return new List<int>();
			}
		}
		/// <summary>
		/// Sets the <see cref="System.String"/> for the specified key.
		/// </summary>
		/// <param name="key">
		/// The key of the configuration entry.
		/// </param>
		/// <param name="val">
		/// The value to set.
		/// </param>
		public void SetString(string key, string val)
		{
			List<string> newList = new List<string>();
			newList.Add(val);
			lock(Lock)
			{
				try
				{
					values[key] = newList;
				}
				catch(KeyNotFoundException)
				{
					values.Add(key, newList);
				}
				Save();
			}
		}
		/// <summary>
		/// Sets the <see cref="System.Int32"/> for the specified key.
		/// </summary>
		/// <param name="key">
		/// The key of the configuration entry.
		/// </param>
		/// <param name="val">
		/// The value to set.
		/// </param>
		public void SetInteger(string key, int val)
		{
			List<string> newList = new List<string>();
			newList.Add(val.ToString());
			lock(Lock)
			{
				try
				{
					values[key] = newList;
				}
				catch(KeyNotFoundException)
				{
					values.Add(key, newList);
				}
				Save();
			}
		}
		/// <summary>
		/// Sets the <see cref="System.Boolean"/> for the specified key.
		/// </summary>
		/// <param name="key">
		/// The key of the configuration entry.
		/// </param>
		/// <param name="val">
		/// The value to set.
		/// </param>
		public void SetBoolean(string key, bool val)
		{
			List<string> newList = new List<string>();
			newList.Add(val ? "true" : "false");
			lock(Lock)
			{
				try
				{
					values[key] = newList;
				}
				catch(KeyNotFoundException)
				{
					values.Add(key, newList);
				}
				Save();
			}
		}
		/// <summary>
		/// Sets the list of <see cref="System.String"/> for the specified key.
		/// </summary>
		/// <param name="key">
		/// The key of the configuration entry.
		/// </param>
		/// <param name="val">
		/// The value to set.
		/// </param>
		public void SetStringList(string key, List<string> list)
		{
			List<string> newList = new List<string>(list);
			lock(Lock)
			{
				try
				{
					values[key] = newList;
				}
				catch(KeyNotFoundException)
				{
					values.Add(key, newList);
				}
				Save();
			}
		}
		/// <summary>
		/// Sets the list of <see cref="System.Int32"/> for the specified key.
		/// </summary>
		/// <param name="key">
		/// The key of the configuration entry.
		/// </param>
		/// <param name="val">
		/// The value to set.
		/// </param>
		public void SetIntegerList(string key, List<int> list)
		{
			List<string> newList = list.ConvertAll<string>(i => i.ToString());
			lock(Lock)
			{
				try
				{
					values[key] = newList;
				}
				catch(KeyNotFoundException)
				{
					values.Add(key, newList);
				}
				Save();
			}
		}
	}
}
