using UnityEngine;
using System.Collections;

public class DataBase
{

    //データベーススクリプト


    //画面切り替え時フェード時間
    private float fadeTime = 1.9f;
    public float FadeTime { get { return fadeTime; } }

    //シャッフルする回数
    private int shuffleNumber = 70;
    public int ShuffleNumber { get { return shuffleNumber; } }

    public enum DayTime
    {
        MORNING = 180,
        NOON = 90,
        EVENING = 0,
        NIGHT = -20
    }

    public enum AudioGroup
    {
        ALL = 0,
        WEREWOLF = 1
    }

    public struct RollExplainTemplete
    {
        public string villager { get; private set; }
        public string diviner { get; private set; }
        public string nurses { get; private set; }
        public string hunter { get; private set; }
        public string werewolf { get; private set; }
        public string madman { get; private set; }
        public string cultleader { get; private set; }
        public string yandere { get; private set; }

        public RollExplainTemplete(string vllgr, string dvnr, string nrss, string hntr, string wrwlf, string mdmn, string cltldr, string yndr)
        {
            villager = vllgr; diviner = dvnr; nurses = nrss; hunter = hntr; werewolf = wrwlf; madman = mdmn; cultleader = cltldr; yandere = yndr;
        }
    }

    //この時間見続けることでクリック処理を行う
    public enum LookTime
    {
        SHORT = 2,
        MIDIUM = 3,
        LONG = 5
    }


    public enum Roll
    {
        VILLAGER,   //村人
        DIVINER,    //占い師
        NURSES,     //霊媒師
        HUNTER,     //狩人
        WEREWOLF,   //人狼
        MADMAN,      //狂人
        CULTLEADER, //カルトリーダー
        YANDERE,	//やんでれ
        UNKNOWN
    }

    public enum Phase
    {
        BRIEFINGROOM,   //briefingルーム
        VRSETTING,      //VRに移行するための準備時間
        ROLLCONFIRM,    //役職確認時間
        MORNING,        //朝の死亡者確認時間
        NOON,           //昼の処刑者相談時間
        VOTETIME,       //昼の投票時間
        EVENING,        //夕方の投票、処刑時間
        NIGHT,           //夜の役職行動時間
        RESULT,         //ゲーム結果
    }

    public enum Camp
    {
        VILLAGE,    //村人陣営
        WEREWOLF,   //人狼陣営
        CULT,       //カルト陣営
        YANDERE		//やんでれ陣営
    }

    /// <summary>
    /// 役職名を返す（日本語）
    /// </summary>
    /// <param name="_roll"></param>
    /// <returns></returns>
    public string RollName(Roll _roll)
    {
        switch (_roll)
        {
            case Roll.VILLAGER:
                return "村人";
            case Roll.DIVINER:
                return "占い師";
            case Roll.HUNTER:
                return "狩人";
            case Roll.NURSES:
                return "霊媒師";
            case Roll.WEREWOLF:
                return "人狼";
            case Roll.MADMAN:
                return "狂人";
            case Roll.CULTLEADER:
                return "カルトリーダー";
            case Roll.YANDERE:
                return "ヤンデレ";
            default:
                return "存在しません";
        }
    }

    public string CampName(Camp _camp)
    {
        switch (_camp)
        {
            case Camp.VILLAGE:
                return "村人陣営";
            case Camp.WEREWOLF:
                return "人狼陣営";
            case Camp.CULT:
                return "カルト陣営（カルトリーダー)";
            case Camp.YANDERE:
                return "ヤンデレ陣営";
            default:
                return "存在しません。";
        }
    }

    /// <summary>
    /// 役職説明を返す
    /// </summary>
    /// <param name="_roll"></param>
    /// <returns></returns>
    public string MyRollExplain(Roll _roll)
    {
        switch (_roll)
        {
            case Roll.VILLAGER:
                return rollExplain.villager;
            case Roll.DIVINER:
                return rollExplain.diviner;
            case Roll.NURSES:
                return rollExplain.nurses;
            case Roll.HUNTER:
                return rollExplain.hunter;
            case Roll.WEREWOLF:
                return rollExplain.werewolf;
            case Roll.MADMAN:
                return rollExplain.madman;
            case Roll.CULTLEADER:
                return rollExplain.cultleader;
            case Roll.YANDERE:
                return rollExplain.yandere;
            default:
                return "";
        }
    }

    public string MyNightActExplain(Roll _roll)
    {
        switch (_roll)
        {
            case Roll.VILLAGER:
                return nightActExplain.villager;
            case Roll.DIVINER:
                return nightActExplain.diviner;
            case Roll.NURSES:
                return nightActExplain.nurses;
            case Roll.HUNTER:
                return nightActExplain.hunter;
            case Roll.WEREWOLF:
                return nightActExplain.werewolf;
            case Roll.MADMAN:
                return nightActExplain.madman;
            case Roll.CULTLEADER:
                return nightActExplain.cultleader;
            case Roll.YANDERE:
                return nightActExplain.yandere;
            default:
                return "";
        }
    }

    public string PhaseName(Phase _phase)
    {
        switch (_phase)
        {
            case Phase.BRIEFINGROOM:
            case Phase.VRSETTING:
                return "準備時間";
            case Phase.ROLLCONFIRM:
                return "役職確認時間";
            case Phase.MORNING:
                return "朝";
            case Phase.NOON:
                return "昼";
            case Phase.VOTETIME:
                return "投票時間";
            case Phase.EVENING:
                return "夕方";
            case Phase.NIGHT:
                return "夜";
            case Phase.RESULT:
                return "結果画面";
            default:
                return "";
        }
    }

    public Roll SearchRollName(string rollName)
    {
        if (rollName == Roll.VILLAGER.ToString())
        {
            return Roll.VILLAGER;
        }
        if (rollName == Roll.DIVINER.ToString())
        {
            return Roll.DIVINER;
        }
        if (rollName == Roll.NURSES.ToString())
        {
            return Roll.NURSES;
        }
        if (rollName == Roll.HUNTER.ToString())
        {
            return Roll.HUNTER;
        }
        if (rollName == Roll.WEREWOLF.ToString())
        {
            return Roll.WEREWOLF;
        }
        if (rollName == Roll.MADMAN.ToString())
        {
            return Roll.MADMAN;
        }
        if (rollName == Roll.CULTLEADER.ToString())
        {
            return Roll.CULTLEADER;
        }
        if (rollName == Roll.YANDERE.ToString())
        {
            return Roll.YANDERE;
        }
        return Roll.UNKNOWN;
    }



    /// <summary>
    /// 朝のテキストを返す
    /// </summary>
    /// <param name="existRaidedPlayer"></param>
    /// <returns></returns>
    public string morningExplain(bool existRaidedPlayer)
    {
        string explainText = "";
        if (existRaidedPlayer)
        {
            explainText = "人狼の脅威が残っているようです。\n話し合いにより人狼を見つけ出してください。";
        }
        else
        {
            explainText = "しかし人狼の脅威は去っていないようです。\n話し合いにより人狼を見つけ出してください。";
        }

        explainText += "\n（カルト教徒になっているか自分のインフォボードも確認下さい。)";

        return explainText;
    }

    /// <summary>
    /// 初日の朝のテキストを返す
    /// </summary>
    /// <returns></returns>
    public string firstMorningExplain()
    {
        return "この村に人狼が紛れているようです。\n村人達で話し合い、人狼を探し処刑してください。";
    }

    private RollExplainTemplete rollExplain = new RollExplainTemplete(
        "能力は特にありません。他の村人の中から人狼を見つけ出し、\n生き延びてください。",
        "夜時間に誰か一人を占い、人狼かどうか\n判別することが出来ます。",
        "昼の時間に処刑された人物が、人狼かどうか\n判別することが出来ます。",
        "夜時間に誰か一人を選択し、人狼から一晩守ることが出来ます。",
        "夜時間に誰か一人を襲撃することが出来ます。\n襲う対象は一晩に一人までです。",
        "能力は特にありません。あなたは人間ですが、\n人狼が生き残るように立ち回らなくてはいけません。",
        "夜時間に一人をカルト信者にし、\nカルトリーダーが生き残りつつ生存者全員をカルト信者に出来れば\nカルトリーダーのみが勝利出来ます。",
        "初日に一人を恋人にし、\n恋人が死亡した翌日に自分も死亡した場合、お互い勝利します。"
        );

    private RollExplainTemplete nightActExplain = new RollExplainTemplete(
        "怪しいと思う人物を誰か一人選択してください。\n※受票数は選択先のプレイヤーのみに表示します。",
        "正体を知りたい対象を選択してください。人狼かどうか判別できます。",
        "怪しいと思う人物を誰か一人選択してください。\n※受票数は選択先のプレイヤーのみに表示します。",
        "人間だと思う対象を一人選択してください。\n今晩人狼から守ることが出来ます。",
        "襲撃したい対象を選択してください。\n人狼が複数いた場合でも襲撃できるのは１人です。",
        "怪しいと思う人物を誰か一人選択してください。\n※受票数は選択先のプレイヤーのみに表示します。",
        "カルト信者にしたい人を一人選択してください。\nカルト信者は\nカルトリーダー間で共有できます。",
        "恋人にしたい相手を選択してください。\n恋人は一人だけ選択できます。"
        );

}
