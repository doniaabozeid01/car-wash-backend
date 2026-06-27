namespace carwash.Service.DTOs.Points;

public enum PointsActionType
{
    Add30,
    Add50,
    Subtract250
}

public static class PointsActionValues
{
    public static int GetChange(PointsActionType action) => action switch
    {
        PointsActionType.Add30 => 30,
        PointsActionType.Add50 => 50,
        PointsActionType.Subtract250 => -250,
        _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
    };
}
