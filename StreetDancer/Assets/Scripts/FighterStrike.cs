﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FighterStrike : MonoBehaviour
{
	public int baseDamage; //Dano base que o golpe causa
    public bool alive = true;
	public List<KeyValuePair<int,int>> comboFlow = new List<KeyValuePair<int, int>>();
	public GameObject targetPlayer;
    public GameObject hitAccuracy;

    public Animator animator;

    public GameObject Hadouken;
    public GameObject Music;
	// Use this for initialization
	void Start () {
		comboFlow.Clear();
	}

	public void Strike(int atk, int perfection)
	// atk = numero do ataque utilizado, perfection: 0 = miss, 1 = bad, 2 = good, 3 = perfect
	{
        //Calcula dano do ataque e aplica no adversario
        if (alive)
        {
            if (atk == 1)
            {
                var had = Instantiate(Hadouken);
                had.transform.position = this.transform.position;
                if (GetComponent<SpriteRenderer>().flipX)
                {
                    had.GetComponent<MoveAttack>().direction = -1;
                    had.GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    had.GetComponent<MoveAttack>().direction = 1;
                }
            }

            if (atk == 3)
            {
                var had = Instantiate(Music);
                had.transform.position = this.transform.position;
                if (GetComponent<SpriteRenderer>().flipX)
                {
                    had.GetComponent<MoveAttack>().direction = -1;
                    had.GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    had.GetComponent<MoveAttack>().direction = 1;
                }
            }

            int atkDamage;
            if (comboFlow.Count < 7)
            {
                atkDamage = baseDamage * perfection / 3;
                if (perfection >= 2)
                {
                    comboFlow.Add(new KeyValuePair<int, int>(atk, perfection));
                }
                else
                {
                    comboFlow.Clear();
                }
                hitAccuracy.GetComponent<AccuracyFade>().ChangeImage(perfection);

                Debug.Log(atk);
                animator.SetInteger("AttackType", atk);
                Debug.Log(animator.GetInteger("AttackType"));
            }
            else
            {
                atkDamage = FinishComboFlow();
                hitAccuracy.GetComponent<AccuracyFade>().ChangeImage(4);
            }
            targetPlayer.GetComponent<FighterLife>().getDamaged(atkDamage);
        }   
	}

	public int FinishComboFlow()
	{
		float mediaPerfection = 0;
		double variance = 0;
		int transicoes;
		List <int> values = new List<int>();
		comboFlow.ForEach(x => mediaPerfection += x.Value);
		mediaPerfection = mediaPerfection / 7;
		comboFlow.ForEach(x => values.Add(x.Key));
		variance = GetVariance(values);
		transicoes = calculaTransicoes(values);
		comboFlow.Clear();
		int dano = (int)Math.Round( transicoes * variance* baseDamage * mediaPerfection/8);
		Debug.Log(dano);
		return dano;
	}

	private int calculaTransicoes(List <int> values)
	{
		HashSet<int> transicoes = new HashSet<int>();
		int i;
		for (i = 1; i < values.Count; i++)
		{
			transicoes.Add((values[i - 1] * values[i - 1] + values[i]));
		}

		return transicoes.Count;
	}
	
	private double GetVariance(List<int> doubleList)
	{
		double average = doubleList.Average();
		double sum = 0;
		int i;
		for (i = 0; i < doubleList.Count; i++)
		{
			sum += (doubleList[i] - average) * (doubleList[i] - average);
		}
		return sum / i;
	}  
}
