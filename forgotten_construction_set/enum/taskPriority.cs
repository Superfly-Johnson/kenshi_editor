using System;
using System.ComponentModel;
namespace forgotten_construction_set
{
	public enum taskPriority
    { 
        [Description("TP_JUST_ACTION_ֻ�Ƕ���")]
        TP_JUST_ACTION,
        [Description("TP_FLUFF_����")]
        TP_FLUFF,
        [Description("TP_NON_URGENT_�ǽ���")]
        TP_NON_URGENT,
        [Description("TP_URGENT_����")]
        TP_URGENT,
        [Description("TP_OBEDIENCE_����")]
        TP_OBEDIENCE,
        [Description("TP_MAX_SIZE_��֪��ɶ��˼")]
        TP_MAX_SIZE
	}
}