using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace YTPlaylistToMP3.Donwload
{
    public class Console : MainWindow
    {
        private readonly TextBlock feedbackBlock;

        public Console(TextBlock feedbackBlock)
        {
            this.feedbackBlock = feedbackBlock;
        }

        public void Write(string message, bool isNewLine = true)
        {
            if(isNewLine)
                this.feedbackBlock.Text += $"\n{message}";
            else
                this.feedbackBlock.Text += message;
        }
        public void Clear()
        {
            this.feedbackBlock.Text = string.Empty;
        }
    }
}
