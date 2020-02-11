using UnityEngine;

public class BattleGroundKeyPlaces : MonoBehaviour
{
    [SerializeField] private GameObject camperPlace;
    [SerializeField] private GameObject centerPlace;
    [SerializeField] private GameObject flagPlace;
    [SerializeField] private GameObject frontPlace;
    [SerializeField] private GameObject powerUpPlace;
    [SerializeField] private GameObject pylonPlace;
    [SerializeField] private GameObject spawnPlace;

    private Team currentTeam;
    public Team CurrentTeam => currentTeam;

    private int currentTeamId;
    public int CurrentTeamId => currentTeamId;

    public void Init(Team team)
    {
        currentTeam = team;
        currentTeamId = team.Id;
    }

    public Vector3 GetPlacePosition(KeyPlaces place)
    {
        switch (place)
        {
            case KeyPlaces.FLAG:
                return ConvertToZeroYVector(flagPlace.transform.position);
            case KeyPlaces.SPAWN:
                return ConvertToZeroYVector(spawnPlace.transform.position);
            case KeyPlaces.CENTER:
                return ConvertToZeroYVector(centerPlace.transform.position);
            case KeyPlaces.FRONT:
                return ConvertToZeroYVector(frontPlace.transform.position);
            case KeyPlaces.PYLON:
                return ConvertToZeroYVector(pylonPlace.transform.position);
            case KeyPlaces.CAMPER:
                return ConvertToZeroYVector(camperPlace.transform.position);
            case KeyPlaces.POWER_UP:
                return ConvertToZeroYVector(powerUpPlace.transform.position);
        }

        return centerPlace.transform.position;
    }

    public KeyPlaces FindNearestPlace(Vector3 pos)
    {
        var distance = Vector3.Distance(ConvertToZeroYVector(pos), ConvertToZeroYVector(flagPlace.transform.position));
        var place = KeyPlaces.FLAG;

        if (Vector3.Distance(ConvertToZeroYVector(pos), ConvertToZeroYVector(spawnPlace.transform.position)) < distance)
        {
            distance = Vector3.Distance(ConvertToZeroYVector(pos), ConvertToZeroYVector(spawnPlace.transform.position));
            place = KeyPlaces.SPAWN;
        }

        if (Vector3.Distance(ConvertToZeroYVector(pos), ConvertToZeroYVector(centerPlace.transform.position)) < distance)
        {
            distance = Vector3.Distance(ConvertToZeroYVector(pos), ConvertToZeroYVector(centerPlace.transform.position));
            place = KeyPlaces.CENTER;
        }

        if (Vector3.Distance(ConvertToZeroYVector(pos), ConvertToZeroYVector(frontPlace.transform.position)) < distance)
        {
            distance = Vector3.Distance(ConvertToZeroYVector(pos), ConvertToZeroYVector(frontPlace.transform.position));
            place = KeyPlaces.FRONT;
        }

        if (Vector3.Distance(ConvertToZeroYVector(pos), ConvertToZeroYVector(pylonPlace.transform.position)) < distance)
        {
            distance = Vector3.Distance(ConvertToZeroYVector(pos), ConvertToZeroYVector(pylonPlace.transform.position));
            place = KeyPlaces.PYLON;
        }

        if (Vector3.Distance(ConvertToZeroYVector(pos), ConvertToZeroYVector(camperPlace.transform.position)) < distance)
        {
            distance = Vector3.Distance(ConvertToZeroYVector(pos), ConvertToZeroYVector(camperPlace.transform.position));
            place = KeyPlaces.CAMPER;
        }

        if (Vector3.Distance(ConvertToZeroYVector(pos), ConvertToZeroYVector(powerUpPlace.transform.position)) < distance)
        {
            distance = Vector3.Distance(ConvertToZeroYVector(pos), ConvertToZeroYVector(powerUpPlace.transform.position));
            place = KeyPlaces.POWER_UP;
        }

        return place;
    }

    public Vector3 ConvertToZeroYVector(Vector3 pos)
    {
        return new Vector3(pos.x,0 ,pos.z);
    }

    public float GetPlaceDistance(Vector3 pos, KeyPlaces place)
    {
        return Vector3.Distance(ConvertToZeroYVector(pos), ConvertToZeroYVector(GetPlacePosition(place)));
    }
}

public enum KeyPlaces
{
    NONE,
    FLAG,
    SPAWN,
    CENTER,
    FRONT,
    PYLON,
    CAMPER,
    POWER_UP
}