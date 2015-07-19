/* movieclips/sounds/text */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Gwent.Models
{
     public class CardSlot
     {
          public Image cardImage { get; set; }
          public Storyboard mcPowerIndicator;

          protected bool imageLoaded = false;
          private int _cardIndex;
          private int _instanceId;
          private string _cardState;
          private bool _activeSelectionEnabled;
          private bool _selected;
          private bool _activateEnabled = true;
          private float _lastShadowRotation = 0;
          protected CardInstance cardInstanceRef = null;

          public const string CardMouseOver = "CardMouseOver";
          public const string CardMouseLeftClick = "CardMouseLeftClick";
          public const string CardMouseRightClick = "CardMouseRightClick";
          public const string CardMouseDoubleClick = "CardMouseDoubleClick";
          public const string CardMouseOut = "CardMouseOut";

          public const string STATE_CAROUSEL = "Carousel";
          public const string STATE_DECK = "deckBuilder";
          public const string STATE_BOARD = "Board";

          public const int BOARD_EFFECT_OFFSET_X = 0;
          public const int BOARD_EFFECT_OFFSET_Y = -18;
          public const int BOARD_SELECTED_Y_OFFSET = -15;

          public const int DESCRIPTION_WIDTH = 243;
          public const int DESCRIPTION_HEIGHT = 114;

          public const int CARD_ORIGIN_HEIGHT = 584;
          public const int CARD_ORIGIN_WIDTH = 309;
          public const int CARD_BOARD_HEIGHT = 120;
          public const int CARD_BOARD_WIDTH = 90;
          public const int CARD_CAROUSEL_HEIGHT = 584;

          public const int FACTION_BANNER_OFFSET_X = 6;
          public const int FACTION_BANNER_OFFSET_Y = 17;

          public const float EFFECT_OFFSET_X = 43.5f;
          public const float EFFECT_OFFSET_Y = 0;

          public const float TYPE_ICON_OFFSET_Y = 167.5f;
          public const float TYPE_ICON_OFFSET_X = 68.5f;
          public const float TYPE_ICON_BOARD_SCALE = 0.36f;

          public const float POWER_ICON_BOARD_SCALE = 0.36f;

          private const float shadowMax = 90;
          private const float shadowDelta = 4;

          //public var mcHitBox:flash.display.MovieClip;
          //public var mcCopyCount:flash.display.MovieClip;
          //public var mcLockedIcon:flash.display.MovieClip;
          //public var mcTypeIcon:flash.display.MovieClip;
          //public var mcTitle:flash.display.MovieClip;
          //public var mcFactionBanner:flash.display.MovieClip;
          //public var mcSmallImageMask:flash.display.MovieClip;
          //public var mcSmallImageContainer:flash.display.MovieClip;
          //public var mcCardImageContainer:flash.display.MovieClip;
          //public var mcCardHighlight:flash.display.MovieClip;
          //public var mcEffectIcon1:flash.display.MovieClip;
          //public var mcDesc:flash.display.MovieClip;
          //protected var cardElementHolder:flash.display.MovieClip;

          public CardSlot()
          {
               _instanceId = -1;
               _cardIndex = -1;
               _cardState = STATE_DECK;
               //visible = false;
               /*if (mcCardHighlight)
               {
                    mcCardHighlight.visible = false;
               }
               if (mcCopyCount)
               {
                    mcCopyCount.visible = false;
               }
               if (mcLockedIcon)
               {
                    mcLockedIcon.visible = false;
               }*/
          }

          public CardInstance cardInstance
          {
               get
               {
                    if (!(_instanceId == -1) && cardInstanceRef == null)
                    {
                         cardInstanceRef = CardManager.getInstance().getCardInstance(instanceId);
                    }
                    return cardInstanceRef;
               }
          }

          public int instanceId
          {
               get
               {
                    return _instanceId;
               }

               set
               {
                    cardInstanceRef = null;
                    _instanceId = value;
                    if (_instanceId != -1)
                    {
                         _cardIndex = cardInstance.templateId;
                         updateCardData();
                    }
               }
          }

          protected void updateCardData()
          {
               CardInstance cardInstance = null;
               CardManager cardManager = CardManager.getInstance();
               if (_instanceId != -1)
               {
                    cardInstance = cardManager.getCardInstance(_instanceId);
                    if (cardInstance != null)
                    {
                         setupCardWithTemplate(cardInstance.templateRef);
                    }
                    else
                    {
                         Console.WriteLine("GFX ---- [ERROR ] ---- tried to get card instance for id: {0} , but could not find it?!", _instanceId);
                    }
               }
               if (_cardIndex != -1)
               {
                    if (cardManager.getCardTemplate(_cardIndex) == null)
                    {
                         //cardManager.addEventListener(CardManager.cardTemplatesLoaded, onCardTemplatesLoaded, false, 0, true);
                         setupCardWithTemplate(CardManager.getInstance().getCardTemplate(cardIndex));//?
                    }
                    else
                    {
                         setupCardWithTemplate(cardManager.getCardTemplate(_cardIndex));
                    }
               }
          }

          //fetch images?
          protected void setupCardWithTemplate(CardTemplate cardTemplate)
          {
               string typeString;
               //string placementTypeString;
               //TextBox title = null;//Title
               Console.WriteLine("GFX - CardSlot setting card up with cardID: " + cardIndex + ", and template: " + cardTemplate.title);
               if (cardTemplate != null)
               {
                    typeString = cardTemplate.getTypeString();

                    /*loadIcon("icons/gwint/" + cardTemplate.imageLoc + ".png");
                    if (mcPowerIndicator) 
                    {
                        if (cardTemplate.index >= 1000) 
                        {
                            mcPowerIndicator.visible = false;
                        }
                        else 
                        {
                            mcPowerIndicator.visible = true;
                            if (red.game.witcher3.utils.CommonUtils.hasFrameLabel(mcPowerIndicator, typeString)) 
                            {
                                mcPowerIndicator.gotoAndStop(typeString);
                            }
                            else 
                            {
                                mcPowerIndicator.gotoAndStop("Default");
                            }
                            mcPowerIndicator.addEventListener(flash.events.Event.ENTER_FRAME, onPowerEnteredFrame, false, 0, true);
                            updateCardPowerText();
                        }
                    }
                    if (mcTypeIcon) 
                    {
                        placementTypeString = cardTemplate.getPlacementTypeString();
                        if (red.game.witcher3.utils.CommonUtils.hasFrameLabel(mcTypeIcon, placementTypeString)) 
                        {
                            mcTypeIcon.gotoAndStop(placementTypeString);
                        }
                        else 
                        {
                            mcTypeIcon.gotoAndStop("None");
                        }
                    }
                    if (mcTitle) 
                    {
                        title = mcTitle.getChildByName("txtTitle") as flash.text.TextField;
                        if (title) 
                        {
                            title.htmlText = cardTemplate.title;
                        }
                        title = mcTitle.getChildByName("txtDesc") as flash.text.TextField;
                        if (title) 
                        {
                            title.htmlText = cardTemplate.description;
                        }
                    }
                    if (mcDesc) 
                    {
                        if (red.game.witcher3.utils.CommonUtils.hasFrameLabel(mcDesc, typeString)) 
                        {
                            mcDesc.gotoAndStop(typeString);
                        }
                        else 
                        {
                            mcDesc.gotoAndStop("Default");
                        }
                    }
                    if (mcFactionBanner) 
                    {
                        if (red.game.witcher3.utils.CommonUtils.hasFrameLabel(mcFactionBanner, cardTemplate.getFactionString())) 
                        {
                            mcFactionBanner.gotoAndStop(cardTemplate.getFactionString());
                        }
                        else 
                        {
                            mcFactionBanner.gotoAndStop("None");
                        }
                    }
                    Console.WriteLine("GFX --- setting up card with effect: " + cardTemplate.getEffectString());
                    if (mcEffectIcon1) 
                    {
                        mcEffectIcon1.gotoAndStop(cardTemplate.getEffectString());
                    }*/
                    //updateCardSetup();
               }
               else
               {
                    throw new ArgumentException("GFX -- Tried to setup a card with an unknown template! --- ");
               }
          }

          public int cardIndex
          {
               get
               {
                    return _cardIndex;
               }

               set
               {
                    if (value != _cardIndex)
                    {
                         _cardIndex = value;
                         if (_cardIndex != -1)
                         {
                              updateCardData();
                         }
                    }
               }
          }

          public string cardState
          {
               get
               {
                    return _cardState;
               }

               set
               {
                    if ((value != null || value != "") && _cardState != value)
                    {
                         _cardState = value;
                         updateCardSetup();
                         updateSelectedVisual();
                    }
               }
          }

          public void updateSelectedVisual()
          {
               /*if (mcCardHighlight) 
               {
                   if (selected && activeSelectionEnabled) 
                   {
                       mcCardHighlight.visible = true;
                   }
                   else 
                   {
                       mcCardHighlight.visible = false;
                   }
               }
               if (cardElementHolder) 
               {
                   if (_cardState == STATE_BOARD && selected && activeSelectionEnabled && !(cardInstance == null) && !(cardInstance.inList == CardManager.CARD_LIST_LOC_GRAVEYARD)) 
                   {
                       cardElementHolder.y = BOARD_SELECTED_Y_OFFSET;
                       mcHitBox.y = BOARD_SELECTED_Y_OFFSET;
                       if (_cardState != STATE_BOARD) 
                       {
                           mcHitBox.height = adjCardHeight + Math.Abs(BOARD_SELECTED_Y_OFFSET);
                       }
                       else 
                       {
                           mcHitBox.height = CARD_BOARD_HEIGHT + Math.Abs(BOARD_SELECTED_Y_OFFSET);
                       }
                   }
                   else 
                   {
                       cardElementHolder.y = 0;
                       mcHitBox.y = 0;
                       if (_cardState != STATE_BOARD) 
                       {
                           mcHitBox.height = adjCardHeight;
                       }
                       else 
                       {
                           mcHitBox.height = CARD_BOARD_HEIGHT;
                       }
                   }
               }*/
          }

          protected int adjCardHeight
          {
               get
               {
                    if (_cardState == STATE_BOARD)
                    {
                         return CARD_BOARD_HEIGHT;
                    }
                    if (_cardState == STATE_DECK)
                    {
                         return 355;
                    }
                    if (_cardState == STATE_CAROUSEL)
                    {
                         return CARD_CAROUSEL_HEIGHT;
                    }
                    return CARD_ORIGIN_HEIGHT;
               }
          }

          protected int adjCardWidth
          {
               get
               {
                    if (_cardState == STATE_BOARD)
                    {
                         return CARD_BOARD_WIDTH;
                    }
                    if (_cardState == STATE_DECK)
                    {
                         return 188;
                    }
                    if (_cardState == STATE_CAROUSEL)
                    {
                         return 309;
                    }
                    return CARD_ORIGIN_WIDTH;
               }
          }

          protected void updateCardSetup()
          {
               //int scale;
               //TextBox text = null;
               if (!imageLoaded)
               {
                    return;
               }
               CardTemplate cardTemplate = CardManager.getInstance().getCardTemplate(_cardIndex);
               int halfH = adjCardHeight / 2;
               int halfW = adjCardWidth / 2;
               int relativeH = adjCardHeight / CARD_CAROUSEL_HEIGHT;
               /*if (mcCopyCount)
               {
                    mcCopyCount.x = 0;
                    mcCopyCount.y = halfH;
               }
               if (mcHitBox)
               {
                    if (_cardState != STATE_BOARD)
                    {
                         mcHitBox.width = adjCardWidth;
                         mcHitBox.height = adjCardHeight;
                    }
                    else
                    {
                         mcHitBox.width = CARD_BOARD_WIDTH;
                         mcHitBox.height = CARD_BOARD_HEIGHT;
                    }
               }
               if (mcPowerIndicator)
               {
                    if (_cardState != STATE_BOARD)
                    {
                         mcPowerIndicator.scaleY = scale = relativeH;
                         mcPowerIndicator.scaleX = scale;
                    }
                    else
                    {
                         mcPowerIndicator.scaleY = scale = POWER_ICON_BOARD_SCALE;
                         mcPowerIndicator.scaleX = scale;
                    }
                    mcPowerIndicator.x = -halfW;
                    mcPowerIndicator.y = -halfH;
               }
               if (mcTypeIcon)
               {
                    if (_cardState != STATE_BOARD)
                    {
                         mcTypeIcon.x = -halfW + TYPE_ICON_OFFSET_X * relativeH;
                         mcTypeIcon.y = -halfH + TYPE_ICON_OFFSET_Y * relativeH;
                         mcTypeIcon.scaleY = scale = relativeH;
                         mcTypeIcon.scaleX = scale;
                    }
                    else
                    {
                         mcTypeIcon.x = 40;
                         mcTypeIcon.y = 32;
                         mcTypeIcon.scaleY = scale = TYPE_ICON_BOARD_SCALE;
                         mcTypeIcon.scaleX = scale;
                    }
               }
               if (mcFactionBanner)
               {
                    if (_cardState == STATE_BOARD || !mcPowerIndicator.visible)
                    {
                         mcFactionBanner.visible = false;
                    }
                    else
                    {
                         mcFactionBanner.visible = true;
                         mcFactionBanner.scaleX = scale = relativeH;
                         mcFactionBanner.scaleY = scale;
                         mcFactionBanner.x = -halfW;
                         mcFactionBanner.y = -halfH;
                    }
               }
               if (mcEffectIcon1)
               {
                    if (_cardState != STATE_BOARD)
                    {
                         mcEffectIcon1.scaleY = scale = relativeH;
                         mcEffectIcon1.scaleX = scale;
                         mcEffectIcon1.x = -halfW + EFFECT_OFFSET_X * relativeH;
                         mcEffectIcon1.y = EFFECT_OFFSET_Y * relativeH;
                    }
                    else
                    {
                         mcEffectIcon1.scaleY = scale = TYPE_ICON_BOARD_SCALE;
                         mcEffectIcon1.scaleX = scale;
                         mcEffectIcon1.x = BOARD_EFFECT_OFFSET_X;
                         mcEffectIcon1.y = halfH + BOARD_EFFECT_OFFSET_Y;
                    }
               }
               if (mcDesc && mcTitle)
               {
                    if (_cardState != STATE_BOARD)
                    {
                         mcTitle.visible = true;
                         mcDesc.visible = true;
                         text = mcTitle.getChildByName("txtTitle") as flash.text.TextField;
                         if (text)
                         {
                              if (_cardState != STATE_CAROUSEL)
                              {
                                   if (_cardState == STATE_DECK)
                                   {
                                        if (!mcPowerIndicator.visible || cardTemplate && cardTemplate.factionIdx == CardTemplate.FactionId_Neutral)
                                        {
                                             text.x = -96;
                                             text.y = -83;
                                             text.width = 178;
                                             text.height = 100;
                                        }
                                        else
                                        {
                                             text.x = -53;
                                             text.y = -83;
                                             text.width = 140;
                                             text.height = 100;
                                        }
                                   }
                              }
                              else if (!mcPowerIndicator.visible || cardTemplate && cardTemplate.factionIdx == CardTemplate.FactionId_Neutral)
                              {
                                   text.x = -149;
                                   text.y = -137;
                                   text.width = 287;
                                   text.height = 79;
                              }
                              else
                              {
                                   text.x = -83;
                                   text.y = -137;
                                   text.width = 223;
                                   text.height = 79;
                              }
                         }
                         text = mcTitle.getChildByName("txtDesc") as flash.text.TextField;
                         if (text)
                         {
                              if (_cardState != STATE_CAROUSEL)
                              {
                                   if (_cardState == STATE_DECK)
                                   {
                                        text.visible = false;
                                   }
                              }
                              else
                              {
                                   text.visible = true;
                                   text.x = -156;
                                   text.y = -65;
                                   text.width = 304;
                                   text.height = 70;
                              }
                         }
                         mcDesc.scaleY = scale = relativeH;
                         mcDesc.scaleX = scale;
                         mcDesc.x = 0;
                         mcDesc.y = halfH;
                         mcTitle.x = 0;
                         mcTitle.y = halfH;
                    }
                    else
                    {
                         mcTitle.visible = false;
                         mcDesc.visible = false;
                    }
               }
               if (mcCardHighlight)
               {
                    mcCardHighlight.scaleX = adjCardWidth / 238;
                    mcCardHighlight.scaleY = adjCardHeight / 450;
               }*/
               updateImagePosAndSize();
          }

          protected void updateImagePosAndSize()
          {
               /*if (!_imageLoader)
               {
                    return;
               }*/
               /*if (_cardState != STATE_BOARD)
               {
                    if (mcCardImageContainer)
                    {
                         mcCardImageContainer.addChild(_imageLoader);
                    }
               }
               else
               {
                    adjCardHeight = 170;
                    if (mcSmallImageContainer)
                    {
                         mcSmallImageContainer.addChild(_imageLoader);
                    }
               }*/
               /*int scale;
               _imageLoader.scaleY = scale = adjCardWidth / CARD_ORIGIN_WIDTH;
               _imageLoader.scaleX = scale;
               _imageLoader.x = -_imageLoader.width / 2;
               _imageLoader.y = -adjCardHeight / 2;*/
          }

          public bool selected
          {
               get
               {
                    return _selected;
               }

               set
               {
                    _selected = value;
               }
          }

          public bool activeSelectionEnabled
          {
               get
               {
                    return _activeSelectionEnabled;
               }

               set
               {
                    _activeSelectionEnabled = value;
               }
          }

          public void setCallbacksToCardInstance(CardInstance card)
          {
               card.powerChangeCallback = onCardPowerChanged;
          }

          protected void onCardPowerChanged()
          {
               if (mcPowerIndicator != null)
               {
                    updateCardPowerText();
               }
          }

          protected void updateCardPowerText()
          {
               Console.WriteLine("updateCardPowerText not implemented yet!");
          }
     }
}
