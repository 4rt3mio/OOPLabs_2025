namespace OOPfl
{
    public class FileManager
    {
        public void Save(string filePath, Canvas canvas, CommandManager commandManager)
        {
            var shapesData = canvas.GetShapes();
            var undoCommands = commandManager.undoneCommands;
            var redoCommands = commandManager.executedCommands;

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Shapes:");
                foreach (var shape in shapesData)
                {
                    writer.WriteLine($"ShapeId:{shape.Id},");
                    if (shape is Circle circle)
                    {
                        writer.WriteLine($"Circle:{circle.StartPosition.X},{circle.StartPosition.Y},{circle.Radius},{circle.FillChar}");
                    }
                    else if (shape is Rectangle rectangle)
                    {
                        writer.WriteLine($"Rectangle:{rectangle.StartPosition.X},{rectangle.StartPosition.Y},{rectangle.Width},{rectangle.Height},{rectangle.FillChar}");
                    }
                    else if (shape is Triangle triangle)
                    {
                        writer.WriteLine($"Triangle:{triangle.StartPosition.X},{triangle.StartPosition.Y},{triangle.BaseLength},{triangle.Height},{triangle.FillChar}");
                    }
                }

                writer.WriteLine("\nUndoCommands:");
                foreach (var command in undoCommands)
                {
                    if (command is AddShapeCommand addShapeCommand)
                    {
                        writer.WriteLine($"AddShape:{addShapeCommand.shapeId}");
                    }
                    else if (command is RemoveShapeCommand removeShapeCommand)
                    {
                        writer.WriteLine($"RemoveShape:{removeShapeCommand.shapeId}");
                    }
                    else if (command is FillBackgroundCommand fillBackgroundCommand)
                    {
                        writer.WriteLine($"FillBackground:{fillBackgroundCommand.shapeId},{fillBackgroundCommand.newSymbol},{fillBackgroundCommand.oldSymbol}");
                    }
                    else if (command is MoveShapeCommand moveShapeCommand)
                    {
                        writer.WriteLine($"MoveShape:{moveShapeCommand.shapeId},{moveShapeCommand.newPosition.X},{moveShapeCommand.newPosition.Y}");
                    }
                }

                writer.WriteLine("\nRedoCommands:");
                foreach (var command in redoCommands)
                {
                    if (command is AddShapeCommand addShapeCommand)
                    {
                        writer.WriteLine($"AddShape:{addShapeCommand.shapeId}");
                    }
                    else if (command is RemoveShapeCommand removeShapeCommand)
                    {
                        writer.WriteLine($"RemoveShape:{removeShapeCommand.shapeId}");
                    }
                    else if (command is FillBackgroundCommand fillBackgroundCommand)
                    {
                        writer.WriteLine($"FillBackground:{fillBackgroundCommand.shapeId},{fillBackgroundCommand.newSymbol},{fillBackgroundCommand.oldSymbol}");
                    }
                    else if (command is MoveShapeCommand moveShapeCommand)
                    {
                        writer.WriteLine($"MoveShape:{moveShapeCommand.shapeId},{moveShapeCommand.newPosition.X},{moveShapeCommand.newPosition.Y}");
                    }
                }
            }
        }

        public void Load(string filePath, Canvas canvas, CommandManager commandManager)
        {
            List<Shape> tempShapes = new List<Shape>();
            Stack<ShapeCommand> tempUndoCommands = new Stack<ShapeCommand>();
            Stack<ShapeCommand> tempRedoCommands = new Stack<ShapeCommand>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                Shape currentShape = null;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (string.IsNullOrEmpty(line))
                        break;

                    if (line.StartsWith("ShapeId:"))
                    {
                        string[] shapeParts = line.Split(',');
                        int shapeId = int.Parse(shapeParts[0].Split(':')[1]);

                        currentShape = GetShapeFromFile(reader, shapeId);
                        if (currentShape != null)
                        {
                            tempShapes.Add(currentShape);
                        }
                    }
                    else if (line.StartsWith("UndoCommands:"))
                    {
                        break;
                    }
                }


                canvas.SetShapes(tempShapes);

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    if (line.StartsWith("UndoCommands:"))
                    {
                        while ((line = reader.ReadLine()) != null && !string.IsNullOrEmpty(line))
                        {
                            ShapeCommand command = CreateCommandFromFile(line, canvas);
                            if (command != null)
                            {
                                tempUndoCommands.Push(command);
                            }
                        }
                    }

                    if (line.StartsWith("RedoCommands:"))
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            ShapeCommand command = CreateCommandFromFile(line, canvas);
                            if (command != null)
                            {
                                tempRedoCommands.Push(command);
                            }
                        }
                    }
                }
            }

            Stack<ShapeCommand> undoCommands = new Stack<ShapeCommand>();
            Stack<ShapeCommand> redoCommands = new Stack<ShapeCommand>();

            while (tempUndoCommands.Count > 0)
            {
                undoCommands.Push(tempUndoCommands.Pop());
            }

            while (tempRedoCommands.Count > 0)
            {
                redoCommands.Push(tempRedoCommands.Pop());
            }

            commandManager.undoneCommands = undoCommands;
            commandManager.executedCommands = redoCommands;
        }

        private Shape GetShapeFromFile(StreamReader reader, int shapeId)
        {
            string line = reader.ReadLine();
            Shape shape = null;

            if (line.StartsWith("Circle:"))
            {
                string[] parts = line.Split(':')[1].Split(',');
                int x = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);
                int radius = int.Parse(parts[2]);
                char fillChar = char.Parse(parts[3]);

                shape = new Circle(shapeId, radius, fillChar, new Coordinates(x, y));
            }
            else if (line.StartsWith("Rectangle:"))
            {
                string[] parts = line.Split(':')[1].Split(',');
                int x = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);
                int width = int.Parse(parts[2]);
                int height = int.Parse(parts[3]);
                char fillChar = char.Parse(parts[4]);

                shape = new Rectangle(shapeId, width, height, fillChar, new Coordinates(x, y));
            }
            else if (line.StartsWith("Triangle:"))
            {
                string[] parts = line.Split(':')[1].Split(',');
                int x = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);
                int baseLength = int.Parse(parts[2]);
                int height = int.Parse(parts[3]);
                char fillChar = char.Parse(parts[4]);

                shape = new Triangle(shapeId, baseLength, height, fillChar, new Coordinates(x, y));
            }

            return shape;
        }

        private ShapeCommand CreateCommandFromFile(string line, Canvas canvas)
        {
            string[] parts = line.Split(':');
            string commandType = parts[0];

            if (commandType == "AddShape")
            {
                int shapeId = int.Parse(parts[1]);
                return new AddShapeCommand(canvas, shapeId);
            }
            else if (commandType == "RemoveShape")
            {
                int shapeId = int.Parse(parts[1]);
                return new RemoveShapeCommand(canvas, shapeId);
            }
            else if (commandType == "FillBackground")
            {
                string[] commandParts = parts[1].Split(',');
                int shapeId = int.Parse(commandParts[0]);
                char newSymbol = char.Parse(commandParts[1]);
                char oldSymbol = char.Parse(commandParts[2]);
                return new FillBackgroundCommand(canvas, shapeId, newSymbol, oldSymbol);
            }
            else if (commandType == "MoveShape")
            {
                string[] commandParts = parts[1].Split(',');
                int shapeId = int.Parse(commandParts[0]);
                int newX = int.Parse(commandParts[1]);
                int newY = int.Parse(commandParts[2]);
                return new MoveShapeCommand(canvas, shapeId, new Coordinates(newX, newY));
            }

            return null;
        }
    }
}