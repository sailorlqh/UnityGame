using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class predictPlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update


public static float predictMovement(List<float> arrY, int length, int dimension, int num) {
        int n = dimension + 1;                 
        float[] arrX = new float[length];
        for(int i = 0; i < length; i++) {
            arrX[i] = i;
        } 
        float[,] Guass=new float[n,n+1];      
        for(int i=0;i<n;i++) {
            int j;
            for(j=0;j<n;j++) {
                Guass[i,j] = SumArr(arrX, j + i, length);
            }
            Guass[i,j] = SumArr(arrX,i,arrY,1,length);          
        }
       float[] coff  = ComputGauss(Guass,n);
       float ans = 0;
       for(int i = 0; i <= dimension; i++) {
        ans += coff[i] * Mathf.Pow(length+num, i);
       }
       return ans;
    }


    public static float SumArr(float[] arr, int n, int length)  {
        float s = 0;
        for (int i = 0; i < length; i++) {
            if (arr[i] != 0 || n != 0)         
                s = s + Mathf.Pow(arr[i], n);
            else
                s = s + 1;
        }
        return s;
    }
    public static float SumArr(float[] arr1, int n1, List<float> arr2, int n2, int length) {
        float s=0;
        for (int i = 0; i < length; i++) {
            if ((arr1[i] != 0 || n1 != 0) && (arr2[i] != 0 || n2 != 0))
                s = s + Mathf.Pow(arr1[i], n1) * Mathf.Pow(arr2[i], n2);
            else
                s = s + 1;
        }
        return s;
 
    }
    public static float[] ComputGauss(float[,] Guass,int n) {
        int i, j;
        int k,m;
        float temp;
        float max;
        float s;
        float[] x = new float[n];
        for (i = 0; i < n; i++)
        	x[i] = 0;
   
        for (j = 0; j < n; j++){
            max = 0;         
            k = j;    
            for (i = j; i < n; i++){
                if (Mathf.Abs(Guass[i, j]) > max){
                    max = Guass[i, j];
                    k = i;
                }
            }

           
            if (k != j){
                for (m = j; m < n + 1; m++){
                    temp = Guass[j, m];
                    Guass[j, m] = Guass[k, m];
                    Guass[k, m] = temp;
                }
            }
            if (0 == max){ 
                return x;
            }
           
            for (i = j + 1; i < n; i++) {
                s = Guass[i, j];
                for (m = j; m < n + 1; m++) {
                    Guass[i, m] = Guass[i, m] - Guass[j, m] * s / (Guass[j, j]);
                }
            }

        }
       
        for (i = n-1; i >= 0; i--) {           
            s = 0;
            for (j = i + 1; j < n; j++) {
                s = s + Guass[i,j] * x[j];
            }
            x[i] = (Guass[i,n] - s) / Guass[i,i];
        }
       return x;
    }
}
