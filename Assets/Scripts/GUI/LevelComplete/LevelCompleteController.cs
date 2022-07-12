using System;
using UnityEngine;

namespace Scripts.GUI.LevelComplete
{
    
    [Serializable]
    public class LevelCompleteController
    {
        public LevelCompleteView view;

        public void Show(int progress)
        {
            string title;
            if (progress == 100) title = "AWESOME!";
            else if (progress >= 70) title = "Great!";
            else if (progress >= 30) title = "Good!";
            else title = "Bad!";
            
            view.title.text = title;
            view.title.color = Level.level.gui.progress.progressColor.Evaluate(progress / 100f);

            view.animator.SetTrigger("isShow");
        }

        public void Hide()
        {
            view.animator.SetTrigger("isHide");
        }
    }
}