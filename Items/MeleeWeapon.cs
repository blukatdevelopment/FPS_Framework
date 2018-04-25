using Godot;
using System;

public class MeleeWeapon : Item, IUse, IWeapon {
  
  const int HealthDamage = 10;
  private Vector3 wieldedPosition;
  private Vector3 forwardPosition;
  
  bool swinging = false;
  float busyDelay = 0f;
  bool busy = false;
  delegate void OnBusyEnd();
  OnBusyEnd busyEndHandler;
  
  
  
  public void Init(){
    
  }
  
  public override void Equip(object wielder){
    this.wielder = wielder;
    this.wieldedPosition = GetTranslation();
    this.forwardPosition = this.wieldedPosition + new Vector3(0, 0, -1);
  }
  
  public override void _Process(float delta){
    if(busy){
      busyDelay -= delta;
      if(busyDelay <= 0f){
        busy = false;
        busyDelay = 0f;
        busyEndHandler();
      }
    }  
  }
  
  public Damage GetBaseDamage(){
    return new Damage(HealthDamage);
  }
  
  public override void Use(Item.Uses use, bool released = false){
    switch(use){
      case Uses.A: Swing(); break;
      case Uses.B: GD.Print("Guard"); break;
    }
  }
  
  public override void OnCollide(object body){
    if(!swinging){
      return;
    }
    IReceiveDamage receiver = body as IReceiveDamage;
    IReceiveDamage wielderDamage = wielder as IReceiveDamage;
    if(receiver != null && receiver != wielderDamage){
      Strike(receiver);
    }
    
    
  }
  
  private void Strike(IReceiveDamage receiver){
    GiveDamage(receiver);
    EndSwing();
  }
  
  void GiveDamage(IReceiveDamage receiver){
    Damage damage = new Damage();
    damage.health = HealthDamage;
    receiver.ReceiveDamage(damage);
  }
  
  private void Swing(){
    if(!busy && !swinging){
      StartSwing();
    }
  }
  
  private void StartSwing(){
    GD.Print("Swing start");
    speaker.PlayEffect(Sound.Effects.FistSwing);
    busy = true;
    busyDelay = 0.5f;
    OnBusyEnd endSwing = EndSwing;
    busyEndHandler = endSwing;
    swinging = true;
    Translation = forwardPosition;
  }
   
  private void EndSwing(){
    GD.Print("Swing end.");
    swinging = false;
    busy = false;
    Translation = wieldedPosition;
  } 
}
