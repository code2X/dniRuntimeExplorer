using ImGuiNET;
using System.Collections.Generic;
using System;
using dniRumtimeExplorer.Utils;

namespace dniRumtimeExplorer.Window
{
    public class TabBarView: ImGuiEx.TabBar
    {
        public class TabItem
        {
            public Type obj = null;
            public bool state = false;

            public TabItem(Type type, bool state)
            {
                this.obj = type;
                this.state = state;
            }
        }

        protected Dictionary<int, TabItem> m_ID2Item = new Dictionary<int, TabItem>();
        int? m_PrevSelectID = null;

        public int TabCount => m_ID2Item.Count;

        #region Event
        public enum ETabEvent
        {
            Opened,
            Closed,
            Removed
        }
        public delegate void TabAction(ETabEvent eTabEvent, Type obj);
        public event TabAction OnTabEvent;
        #endregion

        protected virtual int GetNewVarID()
        {
            int index = 0;
            while (m_ID2Item.ContainsKey(index))
            {
                ++index;
            }

            return index;
        }

        /// <summary>
        /// Open a new tab when the tab name does not exist
        /// </summary>
        public int? OpenTab(Type obj)
        {
            if(Contain(obj))
            {
                return null;
            }
            else
            {
                return OpenNewTab(obj);
            }               
        }

        public bool Contain(Type obj)
        {
            foreach (var pair in m_ID2Item)
            {
                if (pair.Value.obj.Name == obj.Name)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Open a new tab anyway
        /// </summary>
        public int OpenNewTab(Type obj)
        {
            int id = GetNewVarID();
            m_ID2Item.Add(id, new TabItem(obj, true));
            return id;
        }

        public override ImGuiTabBarFlags TabBarFlags => ImGuiTabBarFlags.AutoSelectNewTabs | ImGuiTabBarFlags.Reorderable;

        public override void DrawContent()
        {
            if (m_ID2Item.Count == 0)
                return;

            foreach (var pair in m_ID2Item)
            {
                if (pair.Value.state)
                {
                    TabItemView(pair.Value.obj.Name + "##" + pair.Key, () =>
                     {
                         SelecteTab(pair.Key, pair.Value.obj);
                     }, ref pair.Value.state);
                }
            }

            TabStateProc();
        }

        /// <summary>
        /// Process tab state, remove tab when tab state is set to false 
        /// </summary>
        protected virtual void TabStateProc()
        {
            //Process tab whether closed
            foreach (var pair in m_ID2Item)
            {
                if (pair.Value.state == false)
                {
                    if (RemoveTab(pair.Key))
                    {
                        break;
                    }
                }
            }
        }

        public virtual bool ContainTab(int tabID)
        {
            return m_ID2Item.ContainsKey(tabID);
        }

        /// <summary>
        /// Selecte a tab and notify it opened meanwhile notify previous selected tab closed
        /// </summary>
        protected virtual bool SelecteTab(int tabID, Type obj)
        {
            Assert.IsNotNull(obj);

            if(tabID != m_PrevSelectID)
            {
                if (ContainTab(tabID) == false)
                    return false;

                //Notify closed event frist, so that tab can save the context
                NotifyTabClosed(m_PrevSelectID);
                NotifyTabEvent(ETabEvent.Opened, tabID, obj);

                m_PrevSelectID = tabID;
            }           
            return true;
        }

        /// <summary>
        /// Remove tab and notify it removed if succeed
        /// </summary>
        public virtual bool RemoveTab(int tabID)
        {
            if (m_ID2Item.TryGetValue(tabID, out TabItem removeObject))
            {
                bool res;
                if (res = m_ID2Item.Remove(tabID))
                {
                    NotifyTabEvent(ETabEvent.Removed, tabID, removeObject.obj);
                }
                if (TabCount == 0)
                    m_PrevSelectID = null;
                return res;
            }
            return false;
        }

        protected virtual void NotifyTabClosed(int? tabID)
        {           
            if (tabID != null &&
                m_ID2Item.TryGetValue(tabID.Value, out TabItem CloseObject))
            {
                NotifyTabEvent(ETabEvent.Closed, tabID.Value, CloseObject.obj);
            }
        }

        protected virtual void NotifyTabEvent(ETabEvent eTabEvent, int tabID, Type obj)
        {
            Logger.Info(eTabEvent + " Tab Actoin:  " + obj.Name);
            OnTabEvent?.Invoke(eTabEvent, obj);
        }
    }

}
