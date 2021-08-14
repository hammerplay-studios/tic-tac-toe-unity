// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.26
// 

using Colyseus.Schema;

public partial class TicTacToeRoomState : Schema {
	[Type(0, "map", typeof(MapSchema<Player>))]
	public MapSchema<Player> players = new MapSchema<Player>();

	[Type(1, "boolean")]
	public bool isReady = default(bool);

	[Type(2, "int8")]
	public sbyte currentTurn = default(sbyte);

	[Type(3, "array", typeof(ArraySchema<sbyte>), "int8")]
	public ArraySchema<sbyte> board = new ArraySchema<sbyte>();

	[Type(4, "int8")]
	public sbyte winnerIndex = default(sbyte);
}

