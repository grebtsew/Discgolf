using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityStandardAssets.ImageEffects
{
public class enableToggleComponent : MonoBehaviour
{
  public Antialiasing aa;
  public DepthOfField dof;
  public ColorCorrectionCurves ccc;

  public void toogleAA(){
      aa.enabled = !aa.enabled;
  }
  public void toogleDOF(){
      dof.enabled = !dof.enabled;
  }
  public void toogleCCC(){
      ccc.enabled = !ccc.enabled;
  }
}
}