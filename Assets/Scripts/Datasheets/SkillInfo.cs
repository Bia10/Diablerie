﻿using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SkillInfo
{
    public enum Range
    {
        NoRestrictions,
        Melee,
        Ranged,
        Both
    }

    public string skill;
    public int id = -1;
    public string charClass;
    public string skillDesc;
    public int srvStartFunc;
    public int srvDoFunc;
    [Datasheet.Sequence(length = 71)]
    public string[] unused;
    public string _stsound;
    [Datasheet.Sequence(length = 10)]
    public string[] unused2;
    public string castOverlayId;
    public string clientOverlayA;
    public string clientOverlayB;
    public int clientStartFunc;
    public int clientDoFunc;
    public int clientPrqFunc1;
    public int clientPrqFunc2;
    public int clientPrqFunc3;
    public string clientMissile;
    public string clientMissileA;
    public string clientMissileB;
    public string clientMissileC;
    public string clientMissileD;
    [Datasheet.Sequence(length = 6)]
    public string[] clientCalcs;
    public bool warp;
    public bool immediate;
    public bool enhanceable;
    public int attackRank;
    public bool noAmmo;
    public string _range;
    public int weapSel;
    public string itemTypeA1;
    public string itemTypeA2;
    public string itemTypeA3;
    public string eItemTypeA1;
    public string eItemTypeA2;
    public string itemTypeB1;
    public string itemTypeB2;
    public string itemTypeB3;
    public string eItemTypeB1;
    public string eItemTypeB2;
    public string anim;
    public string seqTrans;
    public string monAnim;
    public int seqNum;
    public string seqInput;
    public bool durability;
    public bool useAttackRate;
    public int lineOfSight;
    public bool targetableOnly;
    public bool searchEnemyXY;
    public bool searchEnemyNear;
    public bool searchOpenXY;
    public int selectProc;
    public bool targetCorpse;
    public bool targetPet;
    public bool targetAlly;
    public bool targetItem;
    [Datasheet.Sequence(length = 77)]
    public string[] unused3;
    public int hitShift;
    public int srcDamage;

    // todo move damage fields to separate structure and share it with MissileInfo
    public int minDamage;
    [Datasheet.Sequence(length = 5)]
    public int[] minDamagePerLevel;
    public int maxDamage;
    [Datasheet.Sequence(length = 5)]
    public int[] maxDamagePerLevel;
    public string damageSymPerCalc;
    public string eType;
    public int eMin;
    [Datasheet.Sequence(length = 5)]
    public int[] minEDamagePerLevel;
    public int eMax;
    [Datasheet.Sequence(length = 5)]
    public int[] maxEDamagePerLevel;
    public string eDamageSymPerCalc;

    public int eLen;
    [Datasheet.Sequence(length = 3)]
    public int[] eLenPerLevel;
    public string eLenSymPerCalc;
    public int aiType;
    public int aiBonus;
    public int costMult;
    public int costAdd;

    [System.NonSerialized]
    public OverlayInfo castOverlay;

    [System.NonSerialized]
    public SoundInfo startSound;

    [System.NonSerialized]
    public Range range;

    public static List<SkillInfo> sheet = Datasheet.Load<SkillInfo>("data/global/excel/Skills.txt");
    static Dictionary<string, SkillInfo> map = new Dictionary<string, SkillInfo>();

    static SkillInfo()
    {
        foreach (var row in sheet)
        {
            if (row.id == -1)
                continue;

            row.castOverlay = OverlayInfo.Find(row.castOverlayId);
            row.startSound = SoundInfo.Find(row._stsound);
            if (row._range == "none")
                row.range = Range.NoRestrictions;
            else if (row._range == "h2h")
                row.range = Range.Melee;
            else if (row._range == "rng")
                row.range = Range.Ranged;
            else if (row._range == "both")
                row.range = Range.Both;
            else
                throw new System.Exception("Unknown skill range " + row._range);
            map.Add(row.skill, row);
        }
    }

    public static SkillInfo Find(string id)
    {
        if (id == null)
            return null;
        return map.GetValueOrDefault(id);
    }

    public bool IsRangeOk(Character self, Character targetCharacter, Vector2 targetPoint)
    {
        if (targetCharacter != null)
        {
            targetPoint = targetCharacter.iso.pos;
        }

        return range == SkillInfo.Range.NoRestrictions ||
            Vector2.Distance(self.iso.pos, targetPoint) <= self.attackRange + self.size / 2 + targetCharacter.size / 2;
    }

    public void Do(Character self, Character targetCharacter, Vector3 target)
    {
        if (srvDoFunc == 27)
        {
            // teleport
            self.InstantMove(target);
        }

        if (srvDoFunc == 1)
        {
            if (IsRangeOk(self, targetCharacter, target))
                targetCharacter.TakeDamage(self.attackDamage, self);
        }
        else if (srvDoFunc == 17)
        {
            // charged bold, bolt sentry
            int boltCount = 7;
            for (int i = 0; i < boltCount; ++i)
            {
                var offset = new Vector3(Random.Range(-boltCount / 2f, boltCount / 2f), Random.Range(-boltCount / 2f, boltCount / 2f));
                Missile.Create(clientMissileA, self.iso.pos, target + offset, self);
            }
        }
        else
        {
            if (clientMissileA != null)
            {
                Missile.Create(clientMissileA, self.iso.pos, target, self);
            }
        }

        if (clientMissile != null)
        {
            Missile.Create(clientMissile, self.iso.pos, target, self);
        }
    }
}
