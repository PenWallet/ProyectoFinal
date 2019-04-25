<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class IPBan extends Model
{
    protected $table = 'IPBans';
    public $primaryKey = 'IP';
    public $incrementing = false;
    public $timestamps = false;

    protected $fillable = [
        'IP', 'untilDate',
    ];

    protected $casts = [
        'untilDate' => 'datetime'
    ];
}
