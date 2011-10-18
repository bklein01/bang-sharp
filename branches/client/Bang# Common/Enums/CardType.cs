// CardType.cs
//  
// Author:  WOnder93 <omosnacek@gmail.com>
// 
// Copyright (c) 2011 Ondrej Mosnáček
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
namespace Bang
{
	/// <summary>
	/// Represents the type of a card.
	/// </summary>
	public enum CardType
	{
		/// <summary>
		/// Represents an unknown card.
		/// </summary>
		Unknown,	
		#region Main Cards
		Bang,
		Missed,
		Beer,
		Saloon,
		WellsFargo,
		Diligenza,
		GeneralStore,
		Panic,
		CatBalou,
		Indians,
		Duel,
		Gatling,
		Mustang,
		Appaloosa,
		Barrel,
		Dynamite,
		Jail,
		Volcanic,
		Schofield,
		Remington,
		Carabine,
		Winchester,
		#endregion
		#region Dodge City
		Dodge,
		Punch,
		Springfield,
		Brawl,
		RagTime,
		Tequila,
		Whisky,
		Hideout,
		Silver,
		Sombrero,
		IronPlate,
		TenGallonHat,
		Bible,
		Canteen,
		Knife,
		Derringer,
		Howitzer,
		Pepperbox,
		BuffaloRifle,
		CanCan,
		Conestoga,
		PonyExpress,
		#endregion
	}
}

