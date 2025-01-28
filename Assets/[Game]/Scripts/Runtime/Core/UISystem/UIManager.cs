using System.Collections.Generic;

namespace Runtime.Core
{
    public class UIManager : Singleton<UIManager>
    {
        private Dictionary<PanelIds, IPanel> _panels = new();
        public void AddPanel(PanelIds id, IPanel panel)
        {
            if (_panels.ContainsKey(id)) return;
            _panels.Add(id, panel);
        }

        public void RemovePanel(PanelIds id) 
        {
            if (!_panels.ContainsKey(id)) return;
            _panels.Remove(id);
        }

        public void OpenPanel(PanelIds id)
        {
            if (_panels.ContainsKey(id))
                _panels[id].OpenPanel();
        }

        public void ClosePanel(PanelIds id)
        {
            if ( _panels.ContainsKey(id))
                _panels[id].ClosePanel();
        }
    }
}