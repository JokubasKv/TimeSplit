using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator
{
    public RoomGenerator()
    {
    }

    public List<RoomNode> GenerateRoomsInSpaces(List<AbstractNode> roomSpaces, float roomBottomCornerModifier, float roomTopCornerMidifier, int roomOffset)
    {
        List<RoomNode> roomList = new List<RoomNode>();

        foreach (var space in roomSpaces)
        {
            Vector2Int newBottomLeftPoint = StructureHelper.GenerateBottomLeftCornerBetween(
                space.BottomLeftCorner, space.TopRightCorner, roomBottomCornerModifier, roomOffset);
            Vector2Int newTopRightPoint = StructureHelper.GenerateTopRightCornerBetween(
                space.BottomLeftCorner, space.TopRightCorner, roomTopCornerMidifier, roomOffset);

            space.BottomLeftCorner = newBottomLeftPoint;
            space.TopRightCorner = newTopRightPoint;
            space.BottomRightCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            space.TopLeftCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);
            roomList.Add((RoomNode)space);
        }

        var leftMostRoom = roomList.OrderBy(x => x.BottomLeftCorner.x).FirstOrDefault();
        var topRightRoom = roomList.OrderByDescending(x => x.TopRightCorner.x).FirstOrDefault();

        leftMostRoom.Type = RoomType.StartingRoom;
        topRightRoom.Type = RoomType.ExitRoom;

        return roomList;
    }
}