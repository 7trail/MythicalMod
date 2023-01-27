using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mythical
{
	public class EnhanceEarth : Item
	{
		// Token: 0x06002329 RID: 9001 RVA: 0x00104A64 File Offset: 0x00102E64
		public EnhanceEarth()
		{
			this.ID = staticID;
			this.category = Item.Category.Offense;
			this.element = ElementType.Earth;
			this.empMod = new BoolVarStatMod(staticID, true, 10);
			this.empMod2 = new BoolVarStatMod(staticID, false, 10);
			this.dmgDecMod = new NumVarStatMod(staticID + "Dec", -0.25f, 10, VarStatModType.Multiplicative, false);
		}

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x0600232A RID: 9002 RVA: 0x00104AD8 File Offset: 0x00102ED8
		public override string ExtraInfo
		{
			get
			{
				return base.PercentToStr(this.dmgDecMod, "-");
			}
		}

		// Token: 0x0600232B RID: 9003 RVA: 0x00104B06 File Offset: 0x00102F06
		public override void Activate()
		{
			this.SetModStatus(true);
			this.SetEventHandlers(true);
		}

		// Token: 0x0600232C RID: 9004 RVA: 0x00104B0F File Offset: 0x00102F0F
		public override void Deactivate()
		{
			this.SetModStatus(false);
			this.SetEventHandlers(false);
		}

		private void SetEventHandlers(bool givenStatus)
		{
			if (!this.SetParentAsPlayer())
			{
				return;
			}
			Player parentPlayer = this.parentPlayer;
			parentPlayer.skillDropEventHandlers = (Player.SkillEventHandler)Delegate.Remove(parentPlayer.skillDropEventHandlers, new Player.SkillEventHandler(this.OnSkillLose));
			Player parentPlayer2 = this.parentPlayer;
			parentPlayer2.skillRemoveEventHandlers = (Player.SkillEventHandler)Delegate.Remove(parentPlayer2.skillRemoveEventHandlers, new Player.SkillEventHandler(this.OnSkillLose));
			Player parentPlayer3 = this.parentPlayer;
			parentPlayer3.skillPickUpEventHandlers = (Player.SkillEventHandler)Delegate.Remove(parentPlayer3.skillPickUpEventHandlers, new Player.SkillEventHandler(this.OnSkillGain));
			Player parentPlayer4 = this.parentPlayer;
			parentPlayer4.assignSkillSlotEventHandlers = (Player.AssignSkillSlotEventHandler)Delegate.Remove(parentPlayer4.assignSkillSlotEventHandlers, new Player.AssignSkillSlotEventHandler(this.OnSkillAssign));
			if (givenStatus)
			{
				Player parentPlayer5 = this.parentPlayer;
				parentPlayer5.skillDropEventHandlers = (Player.SkillEventHandler)Delegate.Combine(parentPlayer5.skillDropEventHandlers, new Player.SkillEventHandler(this.OnSkillLose));
				Player parentPlayer6 = this.parentPlayer;
				parentPlayer6.skillRemoveEventHandlers = (Player.SkillEventHandler)Delegate.Combine(parentPlayer6.skillRemoveEventHandlers, new Player.SkillEventHandler(this.OnSkillLose));
				Player parentPlayer7 = this.parentPlayer;
				parentPlayer7.skillPickUpEventHandlers = (Player.SkillEventHandler)Delegate.Combine(parentPlayer7.skillPickUpEventHandlers, new Player.SkillEventHandler(this.OnSkillGain));
				Player parentPlayer8 = this.parentPlayer;
				parentPlayer8.assignSkillSlotEventHandlers = (Player.AssignSkillSlotEventHandler)Delegate.Combine(parentPlayer8.assignSkillSlotEventHandlers, new Player.AssignSkillSlotEventHandler(this.OnSkillAssign));
			}
			foreach (Player.SkillState givenSkill in this.parentPlayer.assignedSkills)
			{
				if (this.SetEmpStatus(givenSkill, givenStatus))
				{
					break;
				}
			}
		}

		// Token: 0x0600232D RID: 9005 RVA: 0x00104B18 File Offset: 0x00102F18
		private void SetModStatus(bool givenStatus)
		{
			if (!this.SetParentAsPlayer())
			{
				return;
			}
			Player parentPlayer = this.parentPlayer;
			parentPlayer.skillPickUpEventHandlers = (Player.SkillEventHandler)Delegate.Remove(parentPlayer.skillPickUpEventHandlers, new Player.SkillEventHandler(this.OnPickUpOrEmpower));
			Player parentPlayer2 = this.parentPlayer;
			parentPlayer2.skillDropEventHandlers = (Player.SkillEventHandler)Delegate.Remove(parentPlayer2.skillDropEventHandlers, new Player.SkillEventHandler(this.OnDrop));
			Player parentPlayer3 = this.parentPlayer;
			parentPlayer3.skillEmpowerEventHandlers = (Player.SkillEventHandler)Delegate.Remove(parentPlayer3.skillEmpowerEventHandlers, new Player.SkillEventHandler(this.OnPickUpOrEmpower));
			if (givenStatus)
			{
				Player parentPlayer4 = this.parentPlayer;
				parentPlayer4.skillPickUpEventHandlers = (Player.SkillEventHandler)Delegate.Combine(parentPlayer4.skillPickUpEventHandlers, new Player.SkillEventHandler(this.OnPickUpOrEmpower));
				Player parentPlayer5 = this.parentPlayer;
				parentPlayer5.skillDropEventHandlers = (Player.SkillEventHandler)Delegate.Combine(parentPlayer5.skillDropEventHandlers, new Player.SkillEventHandler(this.OnDrop));
				Player parentPlayer6 = this.parentPlayer;
				parentPlayer6.skillEmpowerEventHandlers = (Player.SkillEventHandler)Delegate.Combine(parentPlayer6.skillEmpowerEventHandlers, new Player.SkillEventHandler(this.OnPickUpOrEmpower));
				Player parentPlayer7 = this.parentPlayer;
				string statFieldStr = dmgStr;
				if (cond1 == null)
				{
					cond1 = new Player.SkillApplyCondition(Item.IsEmpowered);
				}
				NumVarStatMod givenMod2 = this.dmgDecMod;
				Player parentPlayer8 = this.parentPlayer;
				string statFieldStr2 = dmgStr;
				if (cond2 == null)
				{
					cond2 = new Player.SkillApplyCondition(Item.IsNotEmpowered);
				}
				Player.ModifySkills(givenMod2, parentPlayer8, statFieldStr2, cond2, true);
			}
			else
			{
				Player.ModifySkills(this.dmgDecMod, this.parentPlayer, dmgStr, null, false);
			}
		}

		// Token: 0x0600232E RID: 9006 RVA: 0x00104CC0 File Offset: 0x001030C0
		private void OnPickUpOrEmpower(Player.SkillState givenSkill)
		{
			if (givenSkill.element == element && givenSkill.element != ElementType.Chaos)
			{
				givenSkill.skillData.RemoveMod(dmgStr, this.dmgDecMod.ID);
			}
			else
			{
				givenSkill.skillData.AddMod(dmgStr, this.dmgDecMod);
			}
		}

		private void OnSkillGain(Player.SkillState givenSkill)
		{
			this.SetEmpStatus(givenSkill, true);
		}

		// Token: 0x06002325 RID: 8997 RVA: 0x001049B8 File Offset: 0x00102DB8
		private void OnSkillLose(Player.SkillState givenSkill)
		{
			this.SetEmpStatus(givenSkill, false);
		}

		// Token: 0x06002326 RID: 8998 RVA: 0x001049C3 File Offset: 0x00102DC3
		private void OnSkillAssign(Player.SkillState givenSkill, int givenSlot)
		{
			this.SetEmpStatus(givenSkill, true);
		}

		// Token: 0x06002327 RID: 8999 RVA: 0x001049D0 File Offset: 0x00102DD0
		private bool SetEmpStatus(Player.SkillState givenSkill, bool givenStatus)
		{
			if (givenSkill == null)
			{
				return false;
			}

			if (givenSkill.element == element && givenStatus)
			{

				givenSkill.SetEmpowered(false, this.empMod2);
				givenSkill.SetEmpowered(true, this.empMod);
				GameUI.RefreshCDUI();
				return true;
			}
			givenSkill.SetEmpowered(false, this.empMod);
			givenSkill.SetEmpowered(true, this.empMod2);

			givenSkill.SetEmpowered(true, this.empMod2);
			GameUI.RefreshCDUI();

			return false;

		}

		// Token: 0x0600232F RID: 9007 RVA: 0x00104D3F File Offset: 0x0010313F
		private void OnDrop(Player.SkillState givenSkill)
		{
			givenSkill.skillData.RemoveMod(dmgStr, this.dmgDecMod.ID);
		}

		// Token: 0x040028C5 RID: 10437
		public static string staticID = "Mythical::EnhanceEarth";

		// Token: 0x040028C6 RID: 10438
		private static string dmgStr = "damage";


		public ElementType element;

		// Token: 0x040028C8 RID: 10440
		private NumVarStatMod dmgDecMod;
		private BoolVarStatMod empMod;
		private BoolVarStatMod empMod2;

		private Player.SkillApplyCondition cond1;
		private Player.SkillApplyCondition cond2;
	}

}

