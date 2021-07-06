using Syncfusion.Windows.Edit;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace DemoREDScript
{
    public class RedscriptLanguage : ProceduralLanguageBase
    {
        private BlockListener currentListener = null;
        private Stack<BlockListener> blocksStack = null;
        private IEnumerable<ILexem> commentsCollection = null;
        private int lastBlockEndLine = 0;


        public RedscriptLanguage(EditControl control) : base(control)
        {
            Name = "Redscript";
            FileExtension = "reds";
            ApplyColoring = true;
            SupportsIntellisense = false; // TODO
            SupportsOutlining = true;
            blocksStack = new Stack<BlockListener>();


        }

        protected override void OnLexemsChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnLexemsChanged(e);
            commentsCollection = this.Lexem.OfType<ILexem>().Where(lex => lex.LexemType == EditTokenType.Comment);
        }

        protected override void InitializeApplyExpandCollapse()
        {
            currentListener = null;
        }

        protected override void ApplyExpandCollapse(ApplyExpandCollapseArgs args)

        {

            bool createListener = false;

            if (args.LanguageBlocks == null)

            {

                return;

            }

            /// LanguageBlocks of args is a collection block definitions added in Lexem property

            /// we use it as identify if the current line contains any block start

            var selblocks = from blk in args.LanguageBlocks

                            where ((!blk.IsRegex && args.ExpandInformation.Text.Trim().StartsWith(blk.BlockStart) || (blk.IsRegex && Regex.Match(args.ExpandInformation.Text, blk.BlockStart).Success)))

                            select blk;

            if (selblocks.Count() > 0)

            {

                var block = selblocks.ElementAt(0);

                ///Checks if the Line starts with a comment or comes under a comment block.

                if (CheckCommentBlock(args.ExpandInformation))

                {

                    createListener = true;

                }

                if (createListener)

                {

                    ///currentListener property is used to hold the current block information. Whenever a block is encountered

                    ///the currentListener value is moved in a stack and a new currentListener is created with the necessary values.

                    if (currentListener != null)

                    {

                        ///IronPython does not have a specific Block end and a condition is checked whether a  class is contained in the current block.

                        if (currentListener.BlockStart.StartsWith("class") && block.BlockStart.StartsWith("class"))

                        {

                            var parentLineItem = args.Source[currentListener.ParentLineNumber - 1];

                            parentLineItem.EndLine = args.Source.IndexOf(args.ExpandInformation) - 1;

                            currentListener.EndLineNumber = parentLineItem.EndLine;

                            lastBlockEndLine = parentLineItem.EndLine;

                        }

                        else

                        {

                            args.ExpandInformation.ParentLineNumber = currentListener.ParentLineNumber;

                            args.ExpandInformation.StartLine = args.Source.IndexOf(args.ExpandInformation) + 2;

                            if (blocksStack == null)

                            {

                                blocksStack = new Stack<BlockListener>();

                            }

                            blocksStack.Push(currentListener);

                        }

                    }

                    currentListener = new BlockListener()

                    {

                        BlockStart = block.BlockStart,

                        BlockEnd = block.BlockEnd,

                        IsPreprocessor = block.IsPreprocessor,

                        ParentLineNumber = args.Source.IndexOf(args.ExpandInformation) + 1,

                        IsRegex = block.IsRegex,

                        CheckParentType = block.CheckParentType,

                        ParentLexemType = block.ParentLexemType,

                        LexemType = block.LexemType,

                        ScopeLevel = block.ScopeLevel

                    };

                    args.ExpandInformation.ContainsPreprocessor = block.IsPreprocessor;

                    args.ExpandInformation.PreprocessorText = block.IsPreprocessor ? args.ExpandInformation.Text.Trim().Substring(block.BlockStart.Length).Trim() : string.Empty;

                    args.ExpandInformation.ContainsLines = true;

                    int tempInd = args.Source.IndexOf(args.ExpandInformation);

                    args.ExpandInformation.StartLine = block.IsPreprocessor ? tempInd + 1 : tempInd + 2;

                }

                else if (currentListener != null)

                {

                    args.ExpandInformation.ParentLineNumber = currentListener.ParentLineNumber;

                    args.ExpandInformation.IsExpanded = true;

                    args.ExpandInformation.ContainsLines = false;

                }

                else

                {

                    args.ExpandInformation.ParentLineNumber = -1;

                    args.ExpandInformation.IsExpanded = true;

                    args.ExpandInformation.ContainsLines = false;

                }

            }

            else if (currentListener != null)

            {

                args.ExpandInformation.ContainsLines = false;

                args.ExpandInformation.ParentLineNumber = currentListener.ParentLineNumber;

                ///Checking if the text is empty to close the block. If there is a block end available we can check the condition and close the block.

                if (args.ExpandInformation.Text.Trim() == string.Empty)

                {

                    var parentLineItem = args.Source[currentListener.ParentLineNumber - 1];

                    parentLineItem.EndLine = args.Source.IndexOf(args.ExpandInformation);

                    currentListener.EndLineNumber = parentLineItem.EndLine;

                    lastBlockEndLine = parentLineItem.EndLine;

                    currentListener = null;

                    if (blocksStack.Count > 0)

                    {

                        currentListener = blocksStack.Pop();

                    }

                }

                else if (currentListener.BlockEnd == null)

                {

                    currentListener = null;

                    if (blocksStack.Count > 0)

                    {

                        currentListener = blocksStack.Pop();

                    }

                }

            }

            else

            {

                args.ExpandInformation.ContainsLines = false;

                if (!args.ExpandInformation.IsExpanded)

                {

                    args.ExpandInformation.IsExpanded = true;

                    args.ExpandInformation.ToggleExpansion = true;

                }

                args.ExpandInformation.IsExpanded = true;

                args.ExpandInformation.ContainsLines = false;

            }



            ///Closing block for last line since there will not be any empty lines

            if (args.Source.IndexOf(args.ExpandInformation) == args.Source.Count - 1)

            {

                if (currentListener != null)

                {

                    var parentLineItem = args.Source[currentListener.ParentLineNumber - 1];

                    parentLineItem.EndLine = lastBlockEndLine;

                    currentListener.EndLineNumber = parentLineItem.EndLine;

                }

            }

        }

        /// <summary>

        /// Override method of ApplyExpandCollapse to perform any initialization before Expand

        ///collapse is applied.

        /// </summary>


        /// <summary>

        /// Helper method to check if the line starts with single line or multiline

        /// comment

        /// </summary>

        /// <param name="item">represents the LineItemExpandInformation instance.</param>

        /// <returns>

        /// a boolean value indicating if the line starts with comment type of lexem.

        /// </returns>

        private bool CheckCommentBlock(LineItemExpandInformation item)

        {

            if (item.LineStartBlock != null && item.LineStartBlock.LexemType == EditTokenType.Comment)

            {

                return true;

            }

            if (commentsCollection != null)

            {

                LineItemExpandInformation tempItem = item;

                RegexOptions options = this.CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

                var blocks = commentsCollection.Where(lexem => (!lexem.IsMultiline && tempItem.Text.Trim().StartsWith(lexem.StartText)) || (lexem.IsMultiline && tempItem.Text.Trim().StartsWith(lexem.StartText) &&

                (tempItem.Text.IndexOf(lexem.EndText) == -1 || tempItem.Text.IndexOf(lexem.EndText) == tempItem.Text.Length - lexem.EndText.Length)));

                if (blocks.Count() > 0)

                {

                    return true;

                }

            }

            return true;

        }
    }
}
