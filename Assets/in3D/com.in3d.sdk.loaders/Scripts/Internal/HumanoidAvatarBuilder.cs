using UnityEngine;

namespace com.in3d.sdk.loaders
{
	internal class HumanoidAvatarBuilder : MonoBehaviour
	{
		private Animator _animator;

		public void Awake()
		{
			_animator = GetComponent<Animator>();
			var description = AvatarUtils.CreateHumanDescription(gameObject);
			var avatar = AvatarBuilder.BuildHumanAvatar(gameObject, description);
			avatar.name = gameObject.name;
			_animator.avatar = avatar;
		}
	}
}