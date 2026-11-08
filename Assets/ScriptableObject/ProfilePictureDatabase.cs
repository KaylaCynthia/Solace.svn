using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ProfilePictureDatabase", menuName = "Scriptable Objects/ProfilePictureDatabase")]
public class ProfilePictureDatabase : ScriptableObject
{
    [SerializeField] List<ProfilePictures> profilePictures;

    public Sprite GetProfilePicture(string profileName)
    {
        foreach (var profile in profilePictures)
        {
            if (profile.profileName == profileName)
            {
                return profile.profileImage;
            }
        }
        return null; // Return null if the profile name is not found
    }
}
