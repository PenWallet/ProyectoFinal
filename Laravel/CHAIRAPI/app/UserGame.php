<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class UserGame extends Model
{
    protected $table = 'UserGames';
    public $primaryKey = ['user', 'game'];
    public $incrementing = false;
    public $timestamps = false;

    protected $fillable = [
        'user', 'game', 'hoursPlayed', 'lastPlayed'
    ];

    protected $casts = [
        'lastPlayed' => 'datetime'
    ];
}
