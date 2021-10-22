using System;
using System.Collections.Generic;
using System.Numerics;

namespace FieldMath
{
	public static class Math
	{
		static Random rand = new Random();
		static class eratosphen
		{
			public static bool isFill { get; private set; } = false;
			static List<int> positiveResheto = new List<int>();
			static List<int> negativeResheto = new List<int>();

			public static void fill(int min, int max)
			{
				int[] a = new int[max + 1 - min];

				for (int i = 0; i < a.Length; i++)
					a[i] = i + min;
				for (int p = 2; p < a.Length; p++)
				{
					if (a[p] != 0)
					{
						if (a[p] % 3 == 1)
							positiveResheto.Add(a[p]);
						else
							negativeResheto.Add(a[p]);

						for (int j = p * p; j < a.Length; j += p)
							a[j] = 0;
					}
				}

				isFill = true;
			}

			public static int getRandomPositiveTrevial()
			{
				return positiveResheto[rand.Next() % positiveResheto.Count];
			}

			public static int getRandomNegativeTrevial()
			{
				return negativeResheto[rand.Next() % negativeResheto.Count];
			}
		}

		static BigInteger nextRandomTrevial(BigInteger min)
		{
			BigInteger ansver;

			ansver = eratosphen.getRandomPositiveTrevial() * eratosphen.getRandomNegativeTrevial() * 2 + 1;

			while (ansver < min)
				switch ((int)(ansver % 3))
				{
					case 1:
						ansver = ansver * eratosphen.getRandomNegativeTrevial() * 2 + 1;
						break;

					case 2:
						ansver = ansver * eratosphen.getRandomPositiveTrevial() * 2 + 1;
						break;

					default:
						ansver = eratosphen.getRandomPositiveTrevial() * eratosphen.getRandomNegativeTrevial() * 2 + 1;
						break;
				}

			return ansver;
		}

		static bool easyCheck(BigInteger subj)
		{
			if (subj % 2 == 0)
				return false;
			if (subj % 3 == 0)
				return false;
			if (subj % 5 == 0)
				return false;
			if (subj % 7 == 0)
				return false;
			if (subj % 11 == 0)
				return false;
			if (subj % 13 == 0)
				return false;

			return true;
		}

		static bool hardCheck(BigInteger subj)
		{
			for (int i = 0; i < 100; i++)
				if (BigInteger.ModPow(bigRandom(0, subj - 1), subj - 1, subj) != 1)
					return false;

			return true;
		}

		public static void init(int min = 0, int max = 7930)
		{
			if (max - min > 1000)
			{
				long sum = min + max;
				min = (int)sum / 2;
				max = min + 8000;
			}
			eratosphen.fill(min, max);
		}

		public static BigInteger bigRandomTrevial(BigInteger min)    //https://habr.com/ru/post/470159/
		{
			if (!eratosphen.isFill)
				init();

			BigInteger ansver;

			do
				do
					ansver = nextRandomTrevial(min);
				while (!easyCheck(ansver));
			while (!hardCheck(ansver));

			return ansver;
		}

		public static BigInteger bigRandom(BigInteger min = default(BigInteger), BigInteger max = default(BigInteger))
		{
			BigInteger ansver = new BigInteger(1);
			for (int i = 0; i < 15; i++)
				ansver *= rand.Next();

			if (min < max)
			{
				while (ansver < min)
					ansver *= rand.Next();

				while (ansver < min || ansver > max)
				{
					if (ansver < min)
						ansver *= rand.Next();
					else if (ansver > max)
						ansver /= 10;
				}
			}

			return ansver;
		}

		public static List<byte> toDouble(this BigInteger number)
		{
			List<byte> ansver = new List<byte>();

			while (number > 0)
			{
				ansver.Add((byte)(number % 2));
				number /= 2;
			}

			return ansver;
		}

		public static BigInteger phi(this BigInteger n)   //http://e-maxx.ru/algo/export_euler_function
		{
			BigInteger res = n;

			for (BigInteger i = 2; i * i <= n; ++i)
				if (n % i == 0)
				{
					while (n % i == 0)
						n /= i;
					res -= res / i;
				}
			if (n > 1)
				res -= res / n;

			return res;
		}

		public static BigInteger memoryEffectiveModPow(BigInteger number, BigInteger power, BigInteger modul)    //https://codetown.ru/plusplus/algoritm-vozvedeniya-v-stepen/
		{
			List<byte> powerInDouble = power.toDouble();

			BigInteger buf = number;
			BigInteger sum = 1;
			if (powerInDouble[0] == 1)
				sum = number;

			for (int i = 1; i < powerInDouble.Count; i++)
			{
				buf = (buf * buf) % modul;  //Вместо массива сумма с буфером
				if (powerInDouble[i] == 1)
					sum *= buf;
			}

			return sum % modul;
		}

		public static BigInteger nod(BigInteger a, BigInteger b)
		{
			return BigInteger.GreatestCommonDivisor(a, b);
		}

		public static BigInteger memoryEffectiveNod(BigInteger a, BigInteger b)    //https://www.wikiwand.com/ru/Алгоритм_Евклида
		{
			BigInteger buf;

			while (b != 0)
			{
				buf = a;
				a = b;
				b = buf % b;
			}

			return a;
		}

		public static BigInteger advancedNod(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y)
		{
			BigInteger q, r, x1 = 0, x2 = 1, y1 = 1, y2 = 0;
			bool isSwap = false;
			if (b == 0)
			{
				x = 1;
				y = 0;

				return a;
			}

			if (a > b)
			{
				q = a;
				a = b;
				b = q;
				isSwap = true;
			}

			while (b > 0)
			{
				q = a / b;
				r = a - q * b;
				x = x2 - q * x1;
				y = y2 - q * y1;
				a = b;
				b = r;
				x2 = x1;
				x1 = x;
				y2 = y1;
				y1 = y;
			}

			if (isSwap)
			{
				y = x2;
				x = y2;
			}
			else
			{
				x = x2;
				y = y2;
			}

			return a;
		}

		public static BigInteger nodByEvclide(BigInteger number, BigInteger modul)
		{
			BigInteger d = new BigInteger(1);
			BigInteger y = new BigInteger(1);

			advancedNod(number, modul, out d, out y);

			while (d < 0)
				d = (d + modul) % (modul);

			return d;
		}

		public static BigInteger nodByBinaryPow(BigInteger number, BigInteger modul)    //Только если модуль простой
		{
			return BigInteger.ModPow(number, modul - 2, modul);
		}

		public static BigInteger primordialRoot(this BigInteger p)   //https://www.wikiwand.com/ru/Первообразный_корень_(теория_чисел)
		{
			List<BigInteger> fact = new List<BigInteger>();
			BigInteger phi = p - 1, n = phi;

			for (BigInteger i = 2; i * i <= n; ++i)
				if (n % i == 0)
				{
					fact.Add(i);
					while (n % i == 0)
						n /= i;
				}
			if (n > 1)
				fact.Add(n);

			for (BigInteger res = 2; res <= p; ++res)
			{
				bool ok = true;
				for (int i = 0; i < fact.Count && ok; ++i)
					ok &= BigInteger.ModPow(res, phi / fact[i], p) != 1;
				if (ok) return res;
			}
			return -1;
		}

		public static BigInteger mutuallyTrivial(this BigInteger number) //Взаимно простое путём перебора, пока НОД != 1
		{
			BigInteger ansver = bigRandom(2, number - 1);

			while (nod(ansver, number) != 1)
				ansver = bigRandom(2, number - 1);

			return ansver;
		}

		public static BigInteger vanilaPow(BigInteger subj, int pow)
		{
			BigInteger ansver = 1;
			for (int i = 0; i < pow; i++)
				ansver *= subj;

			return ansver;
		}

		public static BigInteger[] byteToBigInt(this byte[] inpute, int sizeOfCluster, bool onluNatural = false)
		{
			List<BigInteger> ansver = new List<BigInteger>();

			byte[] buf = new byte[sizeOfCluster];
			int i;
			for (i = 0; i < inpute.Length - sizeOfCluster; i += sizeOfCluster)
			{
				Array.Copy(inpute, i, buf, 0, sizeOfCluster);
				if (onluNatural)
					ansver.Add(buf.toBigInteger());
				else
					ansver.Add(new BigInteger(buf));
			}

			buf = new byte[inpute.Length - i];
			Array.Copy(inpute, i, buf, 0, inpute.Length - i);
			if (onluNatural)
				ansver.Add(buf.toBigInteger());
			else
				ansver.Add(new BigInteger(buf));

			return ansver.ToArray();
		}

		public static byte[] bigIntToByte(this BigInteger[] inpute, int sizeOfCluster, bool onluNatural = false)
		{
			List<byte[]> splitAnsver = new List<byte[]>();

			byte[] buf; byte[] half;
			foreach (BigInteger subj in inpute)
			{
				if (onluNatural)
					buf = subj.toBytes();
				else
					buf = subj.ToByteArray();
				half = new byte[sizeOfCluster];
				Array.Copy(buf, half, buf.Length);
				splitAnsver.Add(half);
			}

			byte[] ansver = new byte[sizeOfCluster * inpute.Length];
			int size = 0;
			foreach (byte[] subj in splitAnsver)
			{
				Array.Copy(subj, 0, ansver, size, sizeOfCluster);
				size += sizeOfCluster;
			}

			return ansver;
		}

		public static byte[] deleteNul(this byte[] inp)
		{
			int count = 0;
			for (int i = inp.Length; i != 0; i--)
				if (inp[i - 1] == 0)
					count++;
				else
					break;

			byte[] ansver = new byte[inp.Length - count];
			Array.Copy(inp, ansver, ansver.Length);
			return ansver;
		}

		public static BigInteger toBigInteger(this byte[] mas)
		{
			BigInteger ansver = 0;
			BigInteger exp = 1;

			foreach (byte subj in mas)
			{
				ansver += subj * exp;
				exp *= 1000;
			}

			return ansver;
		}

		public static byte[] toBytes(this BigInteger bigInteger)
		{
			List<byte> ansver = new List<byte>();

			while (bigInteger > 0)
			{
				ansver.Add((byte)(bigInteger % 1000));
				bigInteger /= 1000;
			}

			return ansver.ToArray();
		}

		public static int coutBytes(this BigInteger bigInteger)
		{
			int ansver = -1;
			while (bigInteger > 0)
			{
				ansver++;
				bigInteger /= 1000;
			}

			return ansver;
		}
	}
}