public static void DetectCommand(string command, string[] sintax, DataGridView Grid)
        {
            if (command.Equals("SELECT"))
            {
                List<string> parameters = new List<string>();
                string tableName;
                string ID;
                string parameter;
                if (sintax.Length>1)
                {
                    if (sintax[1].Equals("*"))
                    {
                        if (sintax.Length > 2)
                        {
                            if (sintax[2].ToUpper().Equals(ReservedWords.ElementAt(1).Value.ToString()))
                            {
                                if (sintax.Length>3)
                                {
                                    tableName = sintax[3];
                                    if (!TableExists(tableName))
                                    {
                                        MessageBox.Show("La tabla "+tableName+" no existe! Sentencia: "+ ReservedWords[command]);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Se seleccionaron todos los datos de la tabla: "+tableName);
                                        TableManagment.SelectAllFields(Grid, tableName);



                                        if (sintax.Length>4)
                                        {
                                            if (IsGO(sintax[4]))
                                            {
                                                SendToDetectCommand(sintax, 5, Grid);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 1; i < sintax.Length; i++)
                        {
                            
                            string[] spaces = sintax[i].Split(' ');
                            if (spaces.Length == 1)
                            {
                                if (spaces[0].EndsWith(","))
                                {
                                    parameters.Add(spaces[0].Trim(','));
                                }
                                else
                                {
                                    if (i+1 < sintax.Length)
                                    {
                                        if (sintax[i+1].ToUpper().Equals(ReservedWords.ElementAt(1).Value.ToString()))
                                        {
                                            parameters.Add(sintax[i]);
                                            if (i+2<sintax.Length)
                                            {
                                                tableName = sintax[i + 2];
                                                if (!TableExists(tableName))
                                                {
                                                    MessageBox.Show("Ta tabla " + tableName + " no existe! Sentencia: " + ReservedWords[command]);
                                                    break;
                                                }
                                                else
                                                {
                                                    if (i+3<sintax.Length)
                                                    {
                                                        if (sintax[i + 3].ToUpper().Equals(ReservedWords.ElementAt(3).Value.ToString()))
                                                        {
                                                            if (i+4 < sintax.Length)
                                                            {
                                                                string[] idConditional = sintax[i + 4].Split(' ');
                                                                if (idConditional.Length == 3)
                                                                {
                                                                    parameter = idConditional[0];
                                                                    if (idConditional[1].Equals("="))
                                                                    {
                                                                        ID = idConditional[2];
                                                                        MessageBox.Show("Se seleccionaron varios datos de la tabla: "+tableName+" con ID: "+ID);

                                                                        #region Selección de Datos
                                                                        TableManagment.Select(Grid, parameters.ToArray(), tableName, int.Parse(ID));

                                                                        #endregion

                                                                        if (sintax.Length > parameters.Count+5)
                                                                        {
                                                                            if (IsGO(sintax[parameters.Count+5]))
                                                                            {
                                                                                i = sintax.Length;
                                                                                SendToDetectCommand(sintax, 6+parameters.Count, Grid);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        MessageBox.Show("Error en la condicional, hace falta el signo = , sentencia: " + ReservedWords[command]);
                                                                        break;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    MessageBox.Show("Error en la condicional, sentencia: "+ReservedWords[command]);
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            i = sintax.Length;
                                                            TableManagment.Select(Grid, parameters.ToArray(), tableName);
                                                            if (parameters.Count+3<sintax.Length)
                                                            {
                                                                if (IsGO(sintax[parameters.Count + 3]))
                                                                {
                                                                    if (parameters.Count + 4 < sintax.Length)
                                                                    {
                                                                        SendToDetectCommand(sintax, (parameters.Count +4),Grid);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        i = sintax.Length;
                                                        TableManagment.Select(Grid, parameters.ToArray(), tableName);
                                                        if (parameters.Count + 3 < sintax.Length)
                                                        {
                                                            if (IsGO(sintax[parameters.Count + 3]))
                                                            {
                                                                if (parameters.Count + 4 < sintax.Length)
                                                                {
                                                                    SendToDetectCommand(sintax, (parameters.Count + 4), Grid);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show($"Hace falta la palabra reservada {ReservedWords["FROM"]}, o hace falta alguna , en los parametros de busqueda! Sentencia: "+ ReservedWords[command]);
                                            break;
                                        }
                                    }
                                }
                            }
                            
                        }
                    }                
                }
            }

            if (command.Equals("DELETE FROM"))
            {
                string tableName = "";
                string ID = "";
                if (sintax.Length>1)
                {
                    tableName = sintax[1];
                    if (TableExists(tableName))
                    {
                        if (sintax.Length > 2)
                        {
                            if (sintax[2].ToUpper().Equals(ReservedWords.ElementAt(3).Value.ToString()))
                            {
                                if (sintax.Length>3)
                                {
                                    string[] elements = sintax[3].Split(' ');
                                    if (elements.Length == 3)
                                    {
                                        if (elements[1] == "=")
                                        {
                                            ID = sintax[3];
                                            //PROCEDER A LA ELIMINACION
                                            #region Se elimina de árbol
                                            ID id = new MicroSQL.ID();
                                            id.id = int.Parse(Regex.Replace(ID, @"[^\d]", ""));
                                            TableManagment.Delete(id, tableName);
                                            #endregion
                                            MessageBox.Show("Se eliminara el dato en la tabla: " + tableName + " con ID: " + ID);
                                            if (sintax.Length>4)
                                            {
                                                if (sintax[4].Equals(ReservedWords.ElementAt(8).Value.ToString()))
                                                {
                                                    if (sintax.Length > 4)
                                                    {
                                                        string[] newSintax = new string[sintax.Length - 5];
                                                        for (int i = 0; i < sintax.Length - (5); i++)
                                                        {
                                                            newSintax[i] = sintax[5 + i];
                                                        }
                                                        DetectCommand(newSintax[0], newSintax, Grid);
                                                    }
                                                    else
                                                    {
                                                        string [] newSintax2 = { "" };
                                                        DetectCommand("", newSintax2, Grid);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Necesita haber un = en la condicion de eliminacion!");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("La condicion de eliminacion no cuenta con el formato adecuado!");
                                    }
                                }
                                
                            }
                            else
                            {
                                TableManagment.Delete(tableName);

                                //ELIMINAR TODO
                                MessageBox.Show("Toda la tabla " + tableName + " fue eliminada!");
                                if (sintax[2].Equals(ReservedWords.ElementAt(8).Value.ToString()))
                                {
                                    if (sintax.Length > 3)
                                    {
                                        string[] newSintax = new string[sintax.Length - 3];
                                        for (int i = 0; i < sintax.Length - (3); i++)
                                        {
                                            newSintax[i] = sintax[3 + i];
                                        }
                                        DetectCommand(newSintax[0], newSintax, Grid);
                                    }
                                }
                            }
                        }
                        else
                        {
                            TableManagment.Delete(tableName);
                            MessageBox.Show("Toda la tabla " + tableName + " fue eliminada!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error, la tabla "+tableName+" no existe, sentencia: "+command);
                    }
                }
            }

            if (command.Equals("CREATE TABLE"))
            {
                string tableName = "";
                string primaryKey = "";
                List<string> listaInt = new List<string>();
                List<string> listaVarchar = new List<string>();
                List<string> listaDateTime = new List<string>();
                List<string> Parametros = new List<string>();
                List<string> Tipos = new List<string>();
                if (sintax.Length>1)
                {
                    tableName = sintax[1];
                    if (sintax.Length>2)
                    {
                        if (sintax[2] != "(")
                        {
                            MessageBox.Show("Sintaxis incorrecta, agregue ( en la sentencia de "+command); 
                        }
                        else
                        {
                            if (sintax.Length > 3)
                            {
                                for (int i = 3; i < sintax.Length; i++)
                                {
                                    string[] spaces = sintax[i].Split(' ');
                                    if (IsReservedOrDataType(spaces[0]))
                                    {
                                        MessageBox.Show("Error de sintaxis, el tipo de dato debe ir despues del nombre del parametro! Sentencia: " + command);
                                    }
                                    else
                                    {
                                        if (spaces.Length == 2)
                                        {
                                            if (sintax[i].EndsWith(","))
                                            {
                                                if (i+1<sintax.Length)
                                                {
                                                    if (sintax[i + 1].Equals(")"))
                                                    {
                                                        MessageBox.Show("Error de sintaxis, no puede haber una coma antes del parentesis de cierre de la tabla! Sentencia: "+command);
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        if (DatatypeExists(spaces[1].Trim(',')))
                                                        {
                                                            if (spaces[1].Trim(',').ToUpper().Equals("INT"))
                                                            {
                                                                listaInt.Add(spaces[0]);
                                                                Tipos.Add("INT");
                                                                Parametros.Add(spaces[0]);
                                                            }
                                                            if (spaces[1].Trim(',').ToUpper().Equals("VARCHAR(100)"))
                                                            {
                                                                listaVarchar.Add(spaces[0]);
                                                                Tipos.Add("VARCHAR");
                                                                Parametros.Add(spaces[0]);
                                                            }
                                                            if (spaces[1].Trim(',').ToUpper().Equals("DATETIME"))
                                                            {
                                                                listaDateTime.Add(spaces[0]);
                                                                Tipos.Add("DATETIME");
                                                                Parametros.Add(spaces[0]);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (i + 1 < sintax.Length)
                                                {
                                                    if (sintax[i + 1] == ")")
                                                    {
                                                        if (spaces[1].ToUpper().Equals("INT"))
                                                        {
                                                            listaInt.Add(spaces[0]);
                                                            Tipos.Add("INT");
                                                            Parametros.Add(spaces[0]);
                                                        }
                                                        if (spaces[1].ToUpper().Equals("VARCHAR(100)"))
                                                        {
                                                            listaVarchar.Add(spaces[0]);
                                                            Tipos.Add("VARCHAR");
                                                            Parametros.Add(spaces[0]);
                                                        }
                                                        if (spaces[1].ToUpper().Equals("DATETIME"))
                                                        {
                                                            listaDateTime.Add(spaces[0]);
                                                            Tipos.Add("DATETIME");
                                                            Parametros.Add(spaces[0]);
                                                        }
                                                        if (primaryKey == "")
                                                        {
                                                            MessageBox.Show("Debe existir una llave primaria dentro de una sentencia " + command);
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            // SE CREA LA TABLA
                                                            if (ValidateTypeArrayLength(listaInt, listaVarchar, listaDateTime))
                                                                TableManagment.CreateTable(Tipos, Parametros, primaryKey, tableName);
                                                            else
                                                                MessageBox.Show("No se permite tener más de 3 elementos por tipo de dato.");

                                                            if (sintax.Length > listaDateTime.Count + listaInt.Count + listaVarchar.Count + 1 + 4)
                                                            {
                                                                int count = listaDateTime.Count + listaInt.Count + listaVarchar.Count + 1 + 4;
                                                                if (sintax[count].ToUpper().Equals(ReservedWords.ElementAt(8).Value.ToString()))
                                                                {
                                                                    if (sintax.Length > listaDateTime.Count + listaInt.Count + listaVarchar.Count + 1 + 4 + 1)
                                                                    {
                                                                        string[] newSintax = new string[sintax.Length - (listaDateTime.Count + listaInt.Count + listaVarchar.Count + 1 + 4 + 1)];
                                                                        if (newSintax.Length >= 2)
                                                                        {
                                                                            for (int j = 0; j < sintax.Length - (count + 1); j++)
                                                                            {
                                                                                newSintax[j] = sintax[count + 1 + j];
                                                                            }
                                                                            i = sintax.Length;
                                                                            DetectCommand(newSintax[0], newSintax, Grid);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Error de sintaxis, debe haber una , al final de los tipos de dato!");
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        if (spaces.Length == 4)
                                        {
                                            string newCommand = spaces[2] + " " + spaces[3].Trim(',');
                                            if (DatatypeExists(newCommand))
                                            {
                                                if (i + 1 < sintax.Length)
                                                {
                                                    if (sintax[i].EndsWith(","))
                                                    {
                                                        if (sintax[i+1] == ")")
                                                        {
                                                            MessageBox.Show("Erro de sintaxis, no debe haber una coma al final de los parametros de la tabla! Sentencia "+command);
                                                            break;
                                                        }
                                                        else
                                                        {

                                                            if (primaryKey == "")
                                                            {
                                                                primaryKey = spaces[0];
                                                            }
                                                            else
                                                            {
                                                                MessageBox.Show("ERROR: solo puede haber una llave primaria!");
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (sintax[i + 1] == ")")
                                                        {
                                                            if (primaryKey == "")
                                                            {
                                                                primaryKey = spaces[0];
                                                                if (ValidateTypeArrayLength(listaInt, listaVarchar, listaDateTime))
                                                                    TableManagment.CreateTable(Tipos, Parametros, primaryKey, tableName);
                                                                else
                                                                    MessageBox.Show("No se permite tener más de 3 elementos por tipo de dato.");

                                                                if (sintax.Length > listaDateTime.Count + listaInt.Count + listaVarchar.Count + 1 + 4)
                                                                {
                                                                    int count = listaDateTime.Count + listaInt.Count + listaVarchar.Count + 1 + 4;
                                                                    if (sintax[count].ToUpper().Equals(ReservedWords.ElementAt(8).Value.ToString()))
                                                                    {
                                                                        if (sintax.Length > listaDateTime.Count + listaInt.Count + listaVarchar.Count + 1 + 4 + 1)
                                                                        {
                                                                            string[] newSintax = new string[sintax.Length - (listaDateTime.Count + listaInt.Count + listaVarchar.Count + 1 + 4 + 1)];
                                                                            if (newSintax.Length >= 2)
                                                                            {
                                                                                for (int j = 0; j < sintax.Length - (count + 1); j++)
                                                                                {
                                                                                    newSintax[j] = sintax[count + 1 + j];
                                                                                }
                                                                                i = sintax.Length;
                                                                                DetectCommand(newSintax[0], newSintax, Grid);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                MessageBox.Show("ERROR: solo puede haber una llave primaria!");
                                                                break;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Error de sintaxis, debe haber una , al final de los tipos de dato!");
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (command.Equals("DROP TABLE"))
            {
                if (sintax.Length > 1)
                {
                    string tableName = sintax[1];
                    if (TableExists(tableName))
                    {
                        File.Delete(Utilities.DefaultPath + Utilities.DefaultTreesFolder + tableName + ".btree");
                        MessageBox.Show("La tabla " + tableName + " fue eliminada exitosamente!");
                        File.Delete(Utilities.DefaultPath + Utilities.DefaultTablesFolder + tableName + ".table");
                    }
                    else
                    {
                        MessageBox.Show("La tabla " + tableName + " no existe, no puede ser eliminada!");
                    }
                }
                if (sintax.Length>2)
                {
                    if (IsGO(sintax[2]))
                    {
                        SendToDetectCommand(sintax, 3,Grid);
                    }
                }
            }

            if (command.Equals("INSERT INTO"))
            {
                string tableName = "";
                List<string> parameters = new List<string>();
                List<string> newValues = new List<string>();
                if (sintax.Length>1)
                {
                    tableName = sintax[1];
                    if (TableExists(tableName))
                    {
                        if (sintax.Length>2)
                        {
                            if (sintax[2] != "(")
                            {
                                MessageBox.Show("Hace falta un '(' en la sintaxis de: "+command);
                            }
                            else
                            {
                                for (int i = 3; i < sintax.Length; i++)
                                {
                                    if (sintax[i].Contains(','))
                                    {
                                        if (sintax[i].Equals(")"))
                                        {
                                            MessageBox.Show("No se permiten las comas al final de la instruccion.");
                                            break;
                                        }else{
                                            parameters.Add(sintax[i].Trim(','));
                                        }
                                    }
                                    else
                                    {
                                        if ((i+1)<sintax.Length)
                                        {
                                            if (sintax[i + 1].Equals(")"))
                                            {
                                                parameters.Add(sintax[i].Trim(','));
                                                i = sintax.Length;
                                            }
                                            else
                                            {
                                                MessageBox.Show("Deben haber comas entre los parametros!");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (parameters.Count > 0)
                        {
                            if (sintax.Length>(parameters.Count+4))
                            {
                                if (sintax[parameters.Count + 4].ToUpper().Equals(ReservedWords.ElementAt(7).Value.ToString()))
                                {
                                    if (sintax.Length >parameters.Count+4)
                                    {
                                        if (!sintax[parameters.Count+5].Equals("("))
                                        {
                                            MessageBox.Show("Hace falta un ( en la sintaxis de "+command);
                                        }
                                        else
                                        {
                                            if (sintax.Length > parameters.Count+5)
                                            {
                                                for (int i = parameters.Count + 6; i < sintax.Length; i++)
                                                {
                                                    if (sintax[i].Contains(','))
                                                    {
                                                        if (sintax[i].Equals(")"))
                                                        {
                                                            MessageBox.Show("No se permiten las comas al final de la instruccion.");
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            newValues.Add(sintax[i].Trim(','));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if ((i + 1) < sintax.Length)
                                                        {
                                                            if (sintax[i + 1].Equals(")"))
                                                            {
                                                                newValues.Add(sintax[i].Trim(','));
                                                                i = sintax.Length;
                                                            }
                                                            else
                                                            {
                                                                MessageBox.Show("Deben haber comas entre los parametros!");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (parameters.Count==newValues.Count)
                                            {                     
                                                if (TableManagment.Insert(newValues, tableName, parameters))
                                                    MessageBox.Show("Datos correctamente agregados a la tabla: " + tableName);

                                                if (sintax.Length>(parameters.Count+newValues.Count+7))
                                                {
                                                    if (sintax[parameters.Count + newValues.Count + 7].Equals(ReservedWords.ElementAt(8).Value.ToString()))
                                                    {
                                                        if (sintax.Length > (parameters.Count + newValues.Count + 7))
                                                        {

                                                        }
                                                        string[] newSintax = new string[sintax.Length - (parameters.Count + newValues.Count + 8)];
                                                        for (int i = 0; i < sintax.Length - (parameters.Count + newValues.Count + 8); i++)
                                                        {
                                                            newSintax[i] = sintax[(parameters.Count + newValues.Count + 8) + i];
                                                        }
                                                        DetectCommand(newSintax[0], newSintax, Grid);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Los parametros y los nuevos valores no coinciden, reviselos y vuelva a intentar!");
                                            }
                                        }
                                    }
                                }
                            } 
                        }
                    }
                    else
                    {
                        MessageBox.Show("La tabla "+tableName+" no existe, sentencia: "+command);
                    }
                }
                else
                {

                }
            }

            if (command.Equals("GO"))
            {

            }
        }