using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordAcademySolver
{
    public class Solver
    {
        #region def
        public char[][] _Original;
        public static CustomRect[][] CRGrid;

        public int[] WordLengths;
        public static int GridSize
        {
            get { return CRGrid.Length; }
        }

        #endregion

        List<string[]> FinalCombinations = new List<string[]>();
        System.Diagnostics.Stopwatch stp = new System.Diagnostics.Stopwatch();
        public Solver()
        {
            Reader.ProcessRaw();
            Initialize();
            PrintGrid(ref CRGrid);
            stp.Start();
            Solve(0, HardCopyGrid(CRGrid));
            Console.WriteLine("\nSearch complete");
            ShowCombinations();
            stp.Stop();
            Console.WriteLine(stp.Elapsed);
            Console.Read();
        }

        public void ShowCombinations()
        {
            foreach (string[] item in FinalCombinations)
            {
                foreach (string i in item)
                {
                    Console.WriteLine(i);
                }
                Console.WriteLine("-------------");
            }
        }

        public void Initialize()
        {
            WordLengths = Reader.WordLengths;
            CRGrid = new CustomRect[Reader.GridSize][];
            _Original = Reader.ProcessedInput;
            for (int i = 0; i < GridSize; i++)
            {
                CRGrid[i] = new CustomRect[GridSize];
                for (int ii = 0; ii < GridSize; ii++)
                {
                    CRGrid[i][ii] = new CustomRect(new Point(i, ii), null, _Original[i][ii]);
                }
            }

            for (int i = 0; i < GridSize; i++)
            {
                for (int ii = 0; ii < GridSize; ii++)
                {
                    CRGrid[i][ii].SearchableNeighbors = CloneSearchableNeighbors(new Point(i, ii), ref CRGrid);
                }
            }
        }

        public void CopyGrid(ref CustomRect[][] target, CustomRect[][] source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                for (int ii = 0; ii < source.Length; ii++)
                {
                    target[i][ii] = source[i][ii];
                }
            }
        }
        public CustomRect[][] HardCopyGrid(CustomRect[][] source)
        {
            CustomRect[][] checkCR = new CustomRect[GridSize][];
            for (int i = 0; i < GridSize; i++)
            {
                checkCR[i] = new CustomRect[GridSize];
                for (int ii = 0; ii < GridSize; ii++)
                {
                    checkCR[i][ii] = source[i][ii].Clone();
                }
            }

            return checkCR;
        }

        public static void UpdateGrid(ref char[][] main)
        {
            //col -> row row row :: col -> row row row
            for (int i = 0; i < main.Length; i++)
            {
                for (int k = main.Length - 1; k >= 1; k--)
                {
                    for (int kk = k - 1; kk >= 0; kk--)
                    {
                        if (main[k][i] == ' ')
                        {
                            main[k][i] = main[kk][i];
                            main[kk][i] = ' ';
                            //break; //stop inside looping
                        }
                    }
                }
            }

        }
        public static void UpdateGrid(ref CustomRect[][] main)
        {
            //col -> row row row :: col -> row row row
            //PrintGrid(ref main);
            for (int i = 0; i < main.Length; i++)
            {
                for (int k = main.Length - 1; k >= 1; k--)
                {
                    for (int kk = k - 1; kk >= 0; kk--)
                    {
                        if (main[k][i].Letter == ' ')
                        {
                            main[k][i].Letter = main[kk][i].Letter;
                            main[kk][i].Letter = ' ';
                            //PrintGrid(ref main);
                            //break; //stop inside looping
                        }
                    }
                }
            }

        }
        public static void PrintGrid(ref char[][] main)
        {
            for (int i = 0; i < main.Length; i++)
            {
                for (int ii = 0; ii < main.Length; ii++)
                {
                    Console.Write(main[i][ii]);
                }
                Console.WriteLine();
            }
        }
        public static void PrintGrid(ref CustomRect[][] main)
        {
            for (int i = 0; i < main.Length; i++)
            {
                for (int ii = 0; ii < main.Length; ii++)
                {
                    Console.Write(main[i][ii].Letter);
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Copies the whole grid then modifies the newly copied grid
        /// </summary>
        /// <param name="queue"></param>
        /// <returns>New modified grid</returns>
        public CustomRect[][] RemoveWordFromGrid(ref CustomRect chainedCR)
        {
            CustomRect[][] checkCR = new CustomRect[GridSize][];
            for (int i = 0; i < GridSize; i++)
            {
                checkCR[i] = new CustomRect[GridSize];
                for (int ii = 0; ii < GridSize; ii++)
                {
                    checkCR[i][ii] = CRGrid[i][ii].Clone();
                }
            }

            do
            {
                checkCR[chainedCR.loc.x][chainedCR.loc.y].Letter = ' ';
                chainedCR = chainedCR.Parent;
            } while (chainedCR != null);
            

            return checkCR;
        }
        public CustomRect[][] RemoveWordFromGrid(ref CustomRect[] queue )
        {
            CustomRect[][] checkCR = new CustomRect[GridSize][];
            for (int i = 0; i < GridSize; i++)
            {
                checkCR[i] = new CustomRect[GridSize];
                for (int ii = 0; ii < GridSize; ii++)
                {
                    checkCR[i][ii] = CRGrid[i][ii].Clone();
                }
            }
            
            foreach (CustomRect item in queue)
            {
                checkCR[item.loc.x][item.loc.y].Letter = ' ';
            }

            return checkCR;
        }
        public CustomRect[][] RemoveWordFromGrid(ref CustomRect chainedCR, ref CustomRect[][] map)
        {
            CustomRect[][] checkCR = new CustomRect[GridSize][];
            for (int i = 0; i < GridSize; i++)
            {
                checkCR[i] = new CustomRect[GridSize];
                for (int ii = 0; ii < GridSize; ii++)
                {
                    checkCR[i][ii] = map[i][ii].Clone();
                }
            }

            do
            {
                checkCR[chainedCR.loc.x][chainedCR.loc.y].Letter = ' ';
                chainedCR = chainedCR.Parent;
            } while (chainedCR != null);


            return checkCR;
        }

        #region Search
        public void _Solve(int contfrom = 0)
        {
            CustomRect CRtmp;
            string str = "";
            PrintGrid(ref _Original);
            string[] wordlist;
            LinkedList<CustomRect> llcr = new LinkedList<CustomRect>();
            List<CustomRect> finalsbackup = new List<CustomRect>();
            CustomRect[][] testgrid = HardCopyGrid(CRGrid); //original
            for (int k = 0; k < WordLengths.Length; k++)
            {
                for (int i = 0; i < GridSize; i++)
                {
                    for (int ii = 0; ii < GridSize; ii++)
                    {
                        llcr.AddLast(CRGrid[i][ii].Clone());
                        TestBFS(llcr, WordLengths[k], finalsbackup);
                        llcr.Clear();
                    }
                }


                if ((wordlist = ListPossibles(Finals.ToArray(), WordLengths[k])).Length == 1)
                {
                    Console.WriteLine("\nSearch complete");
                    return;
                }
                
                RETRY:
                Console.WriteLine("Try each possible word and enter the correct one::");
                Console.Write(">>");
                string result = Console.ReadLine();
                str = @"[a-zA-Z]{" + WordLengths[k] + @"}";
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(str);
                if (regex.IsMatch(result))
                {
                    CRtmp = WordToCustomRect(result, Finals.ToArray());
                    CRGrid = RemoveWordFromGrid(ref CRtmp);
                    UpdateGrid(ref CRGrid);
                    PrintGrid(ref CRGrid);
                    Finals.Clear();
                }
                else goto RETRY;
            }
        }

        bool completeflag = false;
        Stack<string> SolutionStrings = new Stack<string>();
        /// <summary>
        /// Tries solve recursive
        /// </summary>
        /// <param name="contfrom">Wordlength input position</param>
        /// <param name="testg">tmp grid to use</param>
        /// <returns>true: go deeper false: wrong word go back</returns>
        public void Solve(int contfrom, CustomRect[][] testg)
        {
            CustomRect CRtmp;
            string[] wordlist;
            LinkedList<CustomRect> llcr = new LinkedList<CustomRect>();
            List<CustomRect> finalsbackup = new List<CustomRect>();
            CRGrid = testg;
            for (int k = contfrom; k < WordLengths.Length; k++)
            {
                for (int i = 0; i < GridSize; i++)
                {
                    for (int ii = 0; ii < GridSize; ii++)
                    {
                        if (CRGrid[i][ii].Letter == ' ') continue;
                        llcr.AddLast(CRGrid[i][ii].Clone());
                        TestBFS(llcr, WordLengths[k], finalsbackup);
                        llcr.Clear();
                    }
                }

                wordlist = ListPossibles(finalsbackup.ToArray(), WordLengths[k]);

                foreach (string word in wordlist)
                {
                    CRtmp = WordToCustomRect(word, finalsbackup.ToArray());
#if false
                    if (CRtmp == null)
                    {
                        System.Diagnostics.Debugger.Break();
                    } 
#endif
                    CRGrid = RemoveWordFromGrid(ref CRtmp,ref CRGrid);
                    UpdateGrid(ref CRGrid);

                    if (SearchComplete(ref CRGrid))
                    {
                        SolutionStrings.Push(word);
                        completeflag = true;
                        return;
                    }

                    Solve(k + 1, HardCopyGrid(CRGrid));
                    
                    CRGrid = HardCopyGrid(testg); //revert
                    if (completeflag)
                    {
                        SolutionStrings.Push(word);
                        if (k == 0)
                        {
                            ProcessCombination();
                            completeflag = false;
                            continue;
                        }
                        return;
                    }
                }
                return;
            }
        }

        public void ProcessCombination()
        {
            string[] sa = new string[WordLengths.Length];
            for (int i = 0; i < sa.Length; i++)
            {
                sa[i] = SolutionStrings.Pop();
            }
            FinalCombinations.Add(sa);
        }

        public List<CustomRect> Finals = new List<CustomRect>();
        public void TestBFS(LinkedList<CustomRect> visited, int targetlen, List<CustomRect> finals)
        {
            CustomRect tmp = visited.Last();
            visited.RemoveLast();
            if (tmp.Chained == targetlen)
            {
                finals.Add(tmp.Clone());
                return;
            }
            tmp.Visited = true;
            CustomRect[] neighbors;
            neighbors = CloneSearchableNeighbors(tmp.loc, FlattenChain(tmp));
            foreach (CustomRect item in neighbors)
            {
                item.Parent = tmp;
                item.SearchableNeighbors = CloneSearchableNeighbors(item.loc, FlattenChain(item));
                item.UpdateChainValue();
                visited.AddLast(item.Clone());
                TestBFS(visited, targetlen, finals);
            }
        }
        public CustomRect[] GetNeighborClone(Point center)
        {
            List<CustomRect> pts = new List<CustomRect>();
            for (int i = -1; i < 2; i++)
            {
                for (int ii = -1; ii < 2; ii++)
                {

                    if (NeighborValid(center.x - i, center.y - ii, ref CRGrid))
                        if (CRGrid[center.x - i][center.y - ii].loc != center)
                            pts.Add(CRGrid[center.x - i][center.y - ii].Clone());
                }
            }

            return pts.ToArray();
        }
        
        public CustomRect[] CloneSearchableNeighbors(Point center, CustomRect[] queue)
        {
            List<CustomRect> list = new List<CustomRect>(GetNeighborClone(center));
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if ((queue.Contains(list[i])) || (list[i].Visited)) list.RemoveAt(i);
            }
            return list.ToArray();
        }
        public CustomRect[] CloneSearchableNeighbors(Point center, ref CustomRect[][] map)
        {
            List<CustomRect> list = new List<CustomRect>(GetNeighborClone(center));
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Visited) list.RemoveAt(i);
            }
            return list.ToArray();
        }

        private bool InRange(Point p)
        {
            return InRange(p.x) && InRange(p.y);
        }
        private bool InRange(int x)
        {
            return (x >= 0) && (x < GridSize);
        }
        private bool NeighborValid(Point p, ref CustomRect[][] map)
        {
            return NeighborValid(p.x, p.y, ref map);
        }
        private bool NeighborValid(int x, int y, ref CustomRect[][] map)
        {
            Point p = new Point(x, y);
            bool inrange = InRange(p);
            if (!inrange) return false;
            bool notempty = map[x][y].Letter != ' ';

            return (notempty);
        }
        /// <summary>
        /// Checks if this queue will cause the remaining grid to be invalid
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        private bool SearchValid(CustomRect[] queue)
        {
            CustomRect[][] checkCR = RemoveWordFromGrid(ref queue);
            UpdateGrid(ref checkCR);
            bool flag = false;
            for (int i = 0; i < GridSize; i++) //each column
            {
                if (checkCR[GridSize - 1][i].Letter != ' ')
                {
                    flag |= true;
                }
            }
            return flag || SearchComplete(ref checkCR);
            //return true;
        }
        private bool SearchComplete(ref CustomRect[][] grid)
        {
            UpdateGrid(ref grid);
            bool allclear = true;
            for (int i = 0; i < GridSize; i++) //each column
            {
                if (grid[GridSize - 1][i].Letter != ' ')
                {
                    allclear = false;
                }
            }

            return allclear;
        }
        /// <summary>
        /// Returns from deepest to starting point
        /// </summary>
        private CustomRect[] FlattenChain(CustomRect cr)
        {
            Queue<CustomRect> qcr = new Queue<CustomRect>();
            do
            {
                qcr.Enqueue(cr);
                cr = cr.Parent;
            } while (cr != null);
            return qcr.ToArray();
        }
        private void DebugFlatten(CustomRect crs)
        {
            string str = "";
            do
            {
                str += crs.Letter;
                crs = crs.Parent;
            } while (crs != null);
            System.Diagnostics.Debug.WriteLine(Reverse(str));
        }
        System.Text.RegularExpressions.Regex Regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z]+$");
        private string[] ListPossibles(CustomRect[] crs, int len)
        {
            List<string> hs = new List<string>();
            string str = "";
            //Console.WriteLine("Possible words for length of {0}:", len);
            for (int i = 0; i < crs.Length; i++)
            {
                crs[i].UpdateChainValue(); //update just in case
                //if (crs[i].ChainedString.Equals("seam")) System.Diagnostics.Debugger.Break();
                //if (crs[i].ChainedString.Equals("button")) System.Diagnostics.Debugger.Break();
                if (!Regex.IsMatch(crs[i].ChainedString)) continue;
                if (crs[i].ChainedString.Length != len) continue;

                if (!SearchValid(FlattenChain(crs[i])))
                {
                    
                    //DebugFlatten((crs[i]));
                    continue;
                }
                do
                {
                    str += crs[i].Letter;
                    crs[i] = crs[i].Parent;
                } while (crs[i] != null);
                hs.Add(Reverse(str));
                str = "";
            }
            hs = hs.Distinct().ToList();

            //check the word list
            List<string> finals = new List<string>();
            foreach (string word in Reader.WordList)
            {
                if (hs.Contains(word)) finals.Add(word);
            }

#if (debug)
            foreach (string item in finals)
            {
                Console.WriteLine(item);
            }
#endif
            return finals.ToArray();
        }
        public CustomRect WordToCustomRect(string word, CustomRect[] crs)
        {
            string str = "";
            CustomRect backup;
            for (int i = 0; i < crs.Length; i++)
            {
                if (!SearchValid(FlattenChain(crs[i]))) continue;
                backup = crs[i];
                do
                {
                    str += crs[i].Letter;
                    crs[i] = crs[i].Parent;
                } while (crs[i] != null);
                //System.Diagnostics.Debug.WriteLine(Reverse(str));
                if (word.Equals(Reverse(str)))
                {
                    return backup;
                }
                str = "";
            }
            return null;
        }
        /// <summary>
        /// Support method for ListPossibles
        /// </summary>
        private string Reverse(string text)
        {
            if (text == null) return null;

            // this was posted by petebob as well 
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }
#endregion

    }

    public static class Reader
    {
        public static int[] WordLengths;
        public static char[][] ProcessedInput;
        public static HashSet<string> WordList;
        public static string RawInput;
        public static int GridSize
        {
            get { return ProcessedInput.Length; }
        }
        public static bool ReadFromFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(path);
                RawInput = sr.ReadToEnd();
                sr.Close();
                sr = new System.IO.StreamReader("words.txt");
                WordList = new HashSet<string>(sr.ReadToEnd().ToLower().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                sr.Close();
                return true;
            }
            return false;
        }

        public static bool ProcessRaw()
        {
            if (string.IsNullOrWhiteSpace(RawInput)) return false;

            string[] alllines = RawInput.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] wlength = alllines[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            WordLengths = new int[wlength.Length];
            for (int i = 0; i < wlength.Length; i++)
            {
                WordLengths[i] = int.Parse(wlength[i]);
            }
            
            ProcessedInput = new char[alllines.Length - 1][];
            char[] filter = new char[GridSize * GridSize];

            for (int i = 0; i < GridSize; i++)
            {
                ProcessedInput[i] = new char[GridSize];
                char[] line = alllines[i+1].ToCharArray();
                for (int ii = 0; ii < GridSize; ii++)
                {
                    ProcessedInput[i][ii] = char.ToLower(line[ii]);
                    filter[(i*GridSize) + ii] = char.ToLower(line[ii]);
                }
            }

            


            string[] tmp = WordList.ToArray();
            HashSet<int> Wlen = new HashSet<int>(WordLengths);
            int k = 0; //k++ comes first
            char[] tmpchar;
            int count = 0;
            while (k < tmp.Length)
            {
                NEXT:
                tmpchar = tmp[k].ToCharArray();
                if ((count = tmpchar.Except(filter).Count()) > 0) goto CONT; //checks if other chars are used in the word
                foreach (int len in Wlen)
                {
                    if (tmp[k].Length == len)
                    {
                        k++;
                        goto NEXT;
                    }
                }
                CONT:
                WordList.Remove(tmp[k]);
                k++;
            }
            

            

            return true;

        }
        
    }
}
