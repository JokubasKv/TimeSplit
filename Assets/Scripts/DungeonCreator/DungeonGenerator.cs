using System.Collections.Generic;
using System.Linq;

public class DungeonGenerator
{
    List<RoomNode> allRoomNodes = new List<RoomNode>();


    private int dungeonWidth;
    private int dungeonHeight;

    public DungeonGenerator(int dungeonWidth, int dungeonHeight)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonHeight = dungeonHeight;
    }

    internal List<AbstractNode> CalculateRooms(
        int maxIterations,
        int minRoomWidth,
        int minRoomHeight,
        float roomBottomCornerModifier,
        float roomTopCornerMidifier,
        int roomOffset,
        int corridorWidth)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonWidth, dungeonHeight);
        allRoomNodes = bsp.PrepareRoomNodesList(maxIterations, minRoomWidth, minRoomHeight);

        List<AbstractNode> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafs(bsp.RootNode);

        RoomGenerator roomGenerator = new RoomGenerator();
        List<RoomNode> roomList = roomGenerator.GenerateRoomsInSpaces(roomSpaces, roomBottomCornerModifier, roomTopCornerMidifier, roomOffset);

        CorridorsGenerator corridorGenerator = new CorridorsGenerator();
        var corridorList = corridorGenerator.CreateCorridor(allRoomNodes, corridorWidth);

        return new List<AbstractNode>(roomList).Concat(corridorList).ToList();
    }
}
