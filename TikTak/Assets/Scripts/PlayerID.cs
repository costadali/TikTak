
/// <summary>
/// <para> Struct to store player data </para>
/// <para> In a production environment we'd probably be using some sort of ID system, i.e. Their Steam player ID, etc., so I'm creating a PlayerID struct in case we want to store any player related data </para>
/// </summary>
public readonly struct PlayerID
{
	public readonly int Id;
	public readonly string Name;
	public readonly BoardSymbol SymbolToUse;
	public readonly bool IsAI;

	public PlayerID(int id, string name, BoardSymbol symbolToUse, bool isAI)
	{
		Id = id;
		Name = name;
		SymbolToUse = symbolToUse;
		IsAI = isAI;
	}
}