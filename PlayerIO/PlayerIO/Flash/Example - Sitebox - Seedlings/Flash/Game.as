﻿package  {	import flash.display.MovieClip;	import flash.display.LoaderInfo;	import flash.display.Sprite;	import flash.events.MouseEvent;	import flash.utils.setTimeout;	import flash.utils.clearInterval;	import flash.net.navigateToURL;	import flash.net.URLRequest;	import flash.external.ExternalInterface;	import Facebook.FB;	import playerio.*;	import seedlings.*;	public class Game extends MovieClip{		private var shadowContainer:Sprite = new Sprite();		private var flowerContainer:Sprite = new Sprite();		private var flowers:Array = [];		private var shadows:Array = [];		private var friendslist:FriendsList		private var energybar:Energybar		private var levelbar:Levelbar		private var config:DatabaseObject		private var friends:Array = [];				//Type in your information here for debugging and playing the game outside Facebook		private var gameid:String = "[Insert your game id here]"		private var app_id:String = "[Insert your facebook application id here]"		private var show_id:String = "" //Insert a facebook id here if you wish to emulate viewing a player		private var parameters:Object = null		private var isshow:Boolean = false		private var stars:int = 0		private var starscontainer:Sprite = new Sprite()				public function Game() {			//Get flashvars			parameters = LoaderInfo(this.root.loaderInfo).parameters;						//Set default arguments if no parameters is parsed to the game			gameid = parameters.sitebox_gameid || gameid			app_id = parameters.fb_application_id || app_id			show_id = parameters.querystring_id || show_id						isshow = show_id != ""						//If played on facebook			if(parameters.fb_access_token){				//Connect in the background				PlayerIO.quickConnect.facebookOAuthConnect(stage, gameid, parameters.fb_access_token, null, function(c:Client, id:String=""):void{					handleConnect(c, parameters.fb_access_token, id)				}, handleError);			}else{				//Else we are in development, connect with a facebook popup				PlayerIO.quickConnect.facebookOAuthConnectPopup(					stage,					gameid,					"_blank",					[],					null,						//Current PartnerPay partner.					handleConnect, 					handleError				);			}		}						private function handleConnect(c:Client,access_token:String, id:String = ""):void{			trace(">> Connected to Player.IO Webservices")						//If we are in the friendlist invite state. 			if(parameters.post_ids){				//Load player Object				c.bigDB.loadMyPlayerObject(function(o:DatabaseObject):void{					//Award energy					o.energy = 6;					o.save(false, false, function():void{						//On successfull save call javascript method showThanks						if(ExternalInterface.available){							try{								ExternalInterface.call("showThanks")							}catch(e:Error){trace("Do nothing")}						}					}, handleError);				}, handleError)								//Break execution				return;			}						//If not frindlist invite			var loaded:int = 0;			//Init the AS3 Facebook Graph API			FB.init({access_token:access_token, app_id:app_id, debug:true})			//Do a Facebook FQL query for those of my friends whom have the application installed			FB.Data.query('SELECT uid, first_name,last_name, pic_small FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1=me()) AND is_app_user = 1' ).wait(function(rows:*) {								//Create list for ids to load from BigDB				var friendsToLoad:Array = []				//Create facebook data cache object				var facebookdata:Object = {}								//For each entry in FQL query				for( var x:String in rows){					//Store facebook data					facebookdata["fb" + rows[x].uid] = rows[x]					//Insert id to lookup from BigDB					friendsToLoad.push("fb" + rows[x].uid)				}								if(friendsToLoad.length != 0){					//If there are friends to load from BigDB, load them					c.bigDB.loadKeys("PlayerObjects", friendsToLoad, function(frnd:Array):void{						//For each friend						for each(var o:DatabaseObject in frnd){							var friend:DatabaseObject = setDefaults(o)							//Add facebook data to object;							friend.fbdata = facebookdata[o.key]							//Add to friends collection							friends.push(friend);						}						//Init if everything is loaded						if(++loaded==2)	doInit() 					}, handleError)				}else{					//Init if everything is loaded					if(++loaded==2)	doInit() 				}			}, function(e:*){handleError(new Error("From Facebook: " + e))})						if(isshow){				//If the client is showing another user, load that users player object				c.bigDB.load("PlayerObjects", show_id, function(o:DatabaseObject):void{					trace("Loaded 3rd party player")					config = setDefaults(o); //Return populated object					if(++loaded==2)	doInit() 				}, handleError)			}else{				//If we are viewing ourself, load ourself.				c.bigDB.loadMyPlayerObject(function(o:DatabaseObject):void{					trace("Loaded Player Object")					config = setDefaults(o); //Return populated object					if(++loaded==2)	doInit() 				}, handleError)			}		}				private function setDefaults(o:DatabaseObject):DatabaseObject{			//Append default values for the BigDB player object.			o.xp = o.xp || 0;			o.states = o.states || [1,1,1,1,1,1,1,1,1,1,1,1,1]			o.energy = o.energy == undefined ? 6 : o.energy			o.timer = o.timer || new Date().getTime();			return o		}				private function doInit():void{			//Add the game / user interface to the world			addChild(new bg());			addChild(shadowContainer)			addChild(flowerContainer);			shadowContainer.y = flowerContainer.y = 110						energybar = new Energybar(6,config.energy, config.timer);			energybar.x = 5			energybar.y = 5						levelbar = new Levelbar(config.xp, levelUp)			levelbar.x = 230			levelbar.y = 5						//If we are viewing another user			if(isshow){				//Show that user in the top left corner				var returnbtn = new mygamebtn();				returnbtn.addEventListener(MouseEvent.MOUSE_DOWN, function():void{					navigateToURL(new URLRequest(parameters.fb_app_root), "_top")				})				returnbtn.x = stage.stageWidth 				addChild(returnbtn)												for each(var o:DatabaseObject in friends){					if(o.key == show_id){						var viewinfo:FriendsListItem = new FriendsListItem(o.fbdata.first_name + " " + o.fbdata.last_name, o.xp, Levelbar.GetLevel(o.xp) , o.fbdata.pic_small)						viewinfo.x = 5						viewinfo.y = 5						addChild(viewinfo);						break;					}				}			}else{				//Else add Level and Energy bars				addChild(levelbar);				addChild(energybar)			}			//Add friendslist			friendslist = new FriendsList(4, friends, parameters)			addChild(friendslist);			friendslist.y = 380						//Add container for the starts that popup			addChild(starscontainer)						//Add the pots			var ox:int = 120;			var oy:int = 0;			for( var a:int=0;a<13;a++){				var r:Pot = new Pot(a%11,canGrow,spawnStar);								r.x = ox;				r.y = oy;								ox+=130								if(a==3){					ox = 50					oy = 50				}				if(a == 8){					ox = 120					oy = 100				}								r.state = config.states[a]								var s:potshadow = new potshadow();				s.x = r.x-35				s.y = r.y+145				shadowContainer.addChild(s);				shadows.push(s);				flowerContainer.addChild(r);				flowers.push(r);							}						//Toggle pot visibility based on our level			showPots(levelbar.level)		}				private function showPots(level:int):void{			//Toggle pot visibility based on our level			for( var a:int=0;a<flowers.length;a++){				shadows[a].visible = flowers[a].visible = a<level			}		}				//Verify if we can grow our plants				private function canGrow():Boolean{			//We are in show mode, so we can grow nothing			if(isshow ) return false;			//Try growing			var state:Boolean = energybar.useenergy();			if(!state){				//If grow failed, we are out of energy, showInviteFriends dialouge				showInviteFriends()			}			return state		}				//Show invite friends		private function showInviteFriends():void{			//Create object			var oo:MovieClip = addChild(new outOfEnergyDialogue()) as MovieClip;			oo.x = stage.stageWidth / 2;			oo.y = 220			//Handle close click			oo.closebtn.addEventListener(MouseEvent.CLICK, function():void{				removeChild(oo); 			})			//Handle invite click			oo.invitebtn.addEventListener(MouseEvent.CLICK, function():void{				//Load /invite				navigateToURL(new URLRequest(parameters.fb_app_root + "invite"), "_top")			})		}				//We leveled up!		private function levelUp(level:int):void{			//Create sharePopup			var sp:MovieClip = addChild(new sharePopup()) as MovieClip;			sp.x = stage.stageWidth / 2;			sp.y = 220			//Handle close			sp.closebtn.addEventListener(MouseEvent.CLICK, function():void{				removeChild(sp); 			})			//Handle share			sp.sharebtn.addEventListener(MouseEvent.CLICK, function():void{				//Use Facebook AS3 Graph API to do post to the users stream				//Notice that this will only work when the SWF is embedded on a page with the facebook JS API				FB.ui({ 					method: 'stream.publish',					message:'I just gained a new level in Seedlings!',					attachment: { 						name: 'Seedlings, the zen flower garden',						href: 'http://apps.facebook.com/seedlings/',						caption: '{*actor*} just gained a new level in Seedlings.',						description: 'Join us today and get your own flower garden!',						media: [{ 							type: 'image', 							src: 'http://r.playerio.com/r/'+gameid+'/FBSeedlings/img/fblogo.gif', 							href: 'http://apps.facebook.com/seedlings/'						}] 										}				}, function(res:*):void{					//Handle callback from stream.publush					if(res && res.post_id){						//If the post where posted, remove box						removeChild(sp)					}				});			})						//Give user full energy			energybar.giveFull();						//Give user one more pot			showPots(level);		}				//Spawn XP star		private function spawnStar(pot:Pot):void{			//If there is more than 25 stars on stage, return (Prevents the screen from filling up with stars)			if(isshow || stars > 25) return						//Create star			var star:XPStar = new XPStar();			star.x = pot.x+60			star.y = pot.y+240			//Handle star click			star.addEventListener(MouseEvent.CLICK, function():void{				levelbar.giveXP(5);				save();				stars--				starscontainer.removeChild(star);  			})			//Add stars to stage			stars++			starscontainer.addChild(star);		}				var timer:Number = 0;		//Save our state		private function save():void{			//We use a timer to ensure that we never try to save faster than every 500ms			if(timer != 0){				clearInterval(timer);				timer = 0;			}						//Set timer			timer = setTimeout(function():void{				//Set attributes				config.xp = levelbar.xp;				config.energy = energybar.energy;				config.timer = energybar.timer;				for( var a:int=0;a<flowers.length;a++){					config.states[a] = Pot(flowers[a]).state				}				//Save				config.save(false, false, handleSave, handleError)				timer = 0;			},500)		}				private function handleSave():void{			//Saved successfully			trace("Saved to Player.IO")		}				//Handle errors		private function handleError(e:Error):void{			trace("got", e)			var eb:MovieClip = new errorBox()			eb.errortext.text = e			addChild(eb)		}			}	}