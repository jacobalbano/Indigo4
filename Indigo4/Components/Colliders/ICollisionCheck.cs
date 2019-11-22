
using System;

namespace Indigo.Components.Colliders
{
    //  If more than one method exists on this type, the fancy setup logic in Collider will break
    //  Don't forget to update it and test thoroughly
	public interface ICollisionCheck<TOther> where TOther : Collider
	{
		bool CollidesWith(TOther other);
	}
}
